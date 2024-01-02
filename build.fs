open System
open Fake.Core 
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators


//initializeContext()

let combine x y = System.IO.Path.Combine(x,y)

// Build variables
let sln = "benchmarkdotnetanalyser.sln"
let mainProj = ".\src\BenchmarkDotNetAnalyser\BenchmarkDotNetAnalyser.csproj"
let unitTestsProj = "BenchmarkDotNetAnalyser.Tests.Unit.csproj"
let intTestsProj = "BenchmarkDotNetAnalyser.Tests.Integration.csproj"
let publishDir = "publish"
let unitTestDir = "test/BenchmarkDotNetAnalyser.Tests.Unit"
let integrationTestDir = "test/BenchmarkDotNetAnalyser.Tests.Integration"
let integrationTestResultsDir = "BenchmarkDotNetResults"
let sampleBenchmarksDir = "test/BenchmarkDotNetAnalyser.SampleBenchmarks/bin/Release/net8.0"
let sampleBenchmarksResults = "BenchmarkDotNet.Artifacts/results"
let sampleBenchmarksResultsDir = combine sampleBenchmarksDir sampleBenchmarksResults

let unitTestResultsOutputDir = combine unitTestDir "TestResults"
let integrationTestResultsOutputDir = combine integrationTestDir "TestResults"
let strykerOutputDir = combine unitTestDir "StrykerOutput"

let strykerBreak = 69
let strykerHigh = 80
let strykerLow = 70

let runNumber = (match Fake.BuildServer.GitHubActions.Environment.CI false with
                    | true -> Fake.BuildServer.GitHubActions.Environment.RunNumber
                    | _ -> "0")
let commitSha = Fake.BuildServer.GitHubActions.Environment.Sha
let versionSuffix = match Fake.BuildServer.GitHubActions.Environment.Ref with
                    | null 
                    | "refs/heads/main" ->  ""
                    | _ ->                  "-preview"

let version =  sprintf "0.3.%s%s" runNumber versionSuffix
let infoVersion = match commitSha with
                    | null -> version
                    | sha -> sprintf "%s.%s" version sha

sprintf "Ref: %s" Fake.BuildServer.GitHubActions.Environment.Ref |> Trace.log
sprintf "Version: %s" version |> Trace.log
sprintf "Info Version: %s" infoVersion |> Trace.log

let assemblyInfoParams (buildParams)=
    [ ("Version", version); ("AssemblyInformationalVersion", infoVersion) ] |> List.append buildParams   

let packBuildParams (buildParams) =
    [ ("PackageVersion", version); ] |> List.append buildParams   
    
let codeCoverageParams (buildParams)=
    [   ("CollectCoverage", "true"); 
        ("CoverletOutput", "./TestResults/coverage.info"); 
        ( "CoverletOutputFormat", "lcov") ]  |> List.append buildParams

let buildOptions (opts: DotNet.BuildOptions) =            
    { opts with Configuration = DotNet.BuildConfiguration.Release;
                    MSBuildParams = { opts.MSBuildParams with Properties = assemblyInfoParams opts.MSBuildParams.Properties; WarnAsError = Some [ "*" ] } }

let testOptions (opts: DotNet.TestOptions)=
    { opts with NoBuild = false; 
                    Configuration = DotNet.BuildConfiguration.Release; 
                    Logger = Some "trx;LogFileName=test_results.trx";
                    MSBuildParams = { opts.MSBuildParams with Properties = codeCoverageParams opts.MSBuildParams.Properties } }

let packOptions(opts: DotNet.PackOptions)=
    { opts with Configuration = DotNet.BuildConfiguration.Release;
                MSBuildParams = { opts.MSBuildParams with Properties = (packBuildParams opts.MSBuildParams.Properties |> assemblyInfoParams )};
                OutputPath = sprintf ".\\%s\\toolpackage" publishDir |> Some;
        }

let publishOptions(runtime: string)(opts: DotNet.PublishOptions)= 
    { opts with 
       SelfContained = Some true;
       Runtime = Some runtime;       
       OutputPath = Some (sprintf ".\%s\%s" publishDir runtime;);
       MSBuildParams = { opts.MSBuildParams with Properties = assemblyInfoParams opts.MSBuildParams.Properties}
     }

let initTargets() = 
  // Declare build targets
  Target.create "Clean" (fun _ ->   
    Fake.IO.Directory.delete publishDir
    Fake.IO.Directory.create publishDir
    Fake.IO.Directory.delete unitTestResultsOutputDir
    Fake.IO.Directory.delete integrationTestResultsOutputDir
    Fake.IO.Directory.delete strykerOutputDir  
  )

  Target.create "Restore" (fun _ ->    
    DotNet.restore id sln
  )

  Target.create "Build" (fun _ ->
    DotNet.build buildOptions sln 
  )

  Target.create "Unit Tests" (fun _ ->
    let proj = combine unitTestDir unitTestsProj
    DotNet.test testOptions proj
  )

  Target.create "Package" (fun _ -> 
    DotNet.pack packOptions mainProj
  )

  Target.create "Consolidate code coverage" (fun _ ->  
    let args = sprintf @"-reports:""./test/**/coverage.info"" -targetdir:""./%s/codecoverage"" -reporttypes:""HtmlSummary""" publishDir
    let result = DotNet.exec id "reportgenerator" args
    
    if not result.OK then failwithf "reportgenerator failed!"  
  )

  Target.create "Stryker" (fun _ ->  
    let opts (o: DotNet.Options) = { o with WorkingDirectory = unitTestDir }

    let args = sprintf "--threshold-high %i --threshold-low %i -b %i" strykerHigh strykerLow strykerBreak
    let result = DotNet.exec opts "dotnet-stryker" args

    if not result.OK then failwithf "Stryker failed!"  

    
    let strykerFiles = !! (strykerOutputDir + "/**/mutation-report.html") 
    let strykerTargetPath = combine publishDir "stryker"
    
    strykerFiles |> Fake.IO.Shell.copy strykerTargetPath
    sprintf "Stryker reports copied to %s."  strykerTargetPath |> Trace.log
  )

  Target.create "Run Sample benchmarks" (fun _ ->
    
    let opts (o: DotNet.Options) = { o with WorkingDirectory = sampleBenchmarksDir }  
    let args = "-f *"
    let result = DotNet.exec opts "BenchmarkDotNetAnalyser.SampleBenchmarks.dll" args

    if not result.OK then failwithf "Sample benchmarks failed!"
  )

  Target.create "Copy benchmark results" (fun _ -> 

    let sourcePath = combine __SOURCE_DIRECTORY__ sampleBenchmarksResultsDir
    let targetPath = integrationTestResultsDir |> combine integrationTestDir |> combine __SOURCE_DIRECTORY__
        
    Trace.log sourcePath
    Trace.log targetPath

    !! (sourcePath + "/*.csv") |> Fake.IO.Shell.copy targetPath
    !! (sourcePath + "/*-report-full.json") |> Fake.IO.Shell.copy targetPath 
  )

  let runIntegrationTests = (fun _ ->
    let proj = combine integrationTestDir intTestsProj
    DotNet.test testOptions proj
  )

  Target.create "Integration Tests" runIntegrationTests

  Target.create "Integration Tests Standalone" runIntegrationTests

  Target.create "RebuildTestDataValidate" (fun _ -> Trace.log "Done." )

  Target.create "BuildTestAndPackage" (fun _ -> Trace.log "Done." )

  // Declare build dependencies
  "Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Unit Tests"
    ==> "Integration Tests"
    ==> "Consolidate code coverage"
    ==> "Package"
    ==> "BuildTestAndPackage"  
    |> ignore

  "Build"
    ==> "Run Sample benchmarks" 
    ==> "Copy benchmark results"
    ==> "Integration Tests Standalone"
    ==> "RebuildTestDataValidate"
    |> ignore

  "Build"
    ==> "Stryker"
    |> ignore


[<EntryPoint>]
let main argv = 
  argv
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext

  initTargets ()

  Target.runOrDefaultWithArguments "BuildTestAndPackage"
  
  0
