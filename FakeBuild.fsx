#r "paket:
nuget Fake.IO.FileSystem
nuget Fake.DotNet.Cli
nuget Fake.DotNet.MSBuild
nuget Fake.BuildServer.GitHubActions
nuget Fake.Core.Target //"
#if !FAKE
  #load "./.fake/fakebuild.fsx/intellisense.fsx"
#endif

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet

// Build variables
let sln = "benchmarkdotnetanalyser.sln"
let mainProj = ".\src\BenchmarkDotNetAnalyser\BenchmarkDotNetAnalyser.csproj"
let publishDir = "publish"
let testDir = "test/BenchmarkDotNetAnalyser.Tests.Unit"
let testResultsOutputDir = testDir + "/TestResults"
let strykerOutputDir = testDir + "/StrykerOutput"


let runNumber = (match Fake.BuildServer.GitHubActions.Environment.CI false with
                    | true -> Fake.BuildServer.GitHubActions.Environment.RunNumber
                    | _ -> "0")
let commitSha = Fake.BuildServer.GitHubActions.Environment.Sha
let versionSuffix = match Fake.BuildServer.GitHubActions.Environment.Ref with
                    | null -> ""
                    | _ -> "-preview"
let version =  sprintf "0.1.%s%s" runNumber versionSuffix
let infoVersion = match commitSha with
                    | null -> version
                    | sha -> sprintf "%s %s" version sha
 

sprintf "Version: %s" version |> Trace.log
sprintf "Info Version: %s" infoVersion |> Trace.log

let msBuildParams (buildParams)=
    [ ("Version", version); ("AssemblyInformationalVersion", infoVersion) ] |> List.append buildParams   

let packBuildParams (buildParams) =
    [ ("PackageVersion", version); ] |> List.append buildParams   
    
let buildOptions (opts: DotNet.BuildOptions) =            
    { opts with Configuration = DotNet.BuildConfiguration.Release;
                MSBuildParams = { opts.MSBuildParams with Properties = msBuildParams opts.MSBuildParams.Properties}
                }

let testOptions (opts: DotNet.TestOptions)=
    { opts with NoBuild = true; Configuration = DotNet.BuildConfiguration.Release; Logger = Some "trx;LogFileName=unit_test_results.trx" }

let packOptions(opts: DotNet.PackOptions)=
    { opts with Configuration = DotNet.BuildConfiguration.Release;
                MSBuildParams = { opts.MSBuildParams with Properties = (packBuildParams opts.MSBuildParams.Properties |> msBuildParams )};
                OutputPath = sprintf ".\\%s\\toolpackage" publishDir |> Some;
        }

let publishOptions(runtime: string)(opts: DotNet.PublishOptions)= 
    { opts with 
       SelfContained = Some true;
       Runtime = Some runtime;       
       OutputPath = Some (sprintf ".\%s\%s" publishDir runtime;);
       MSBuildParams = { opts.MSBuildParams with Properties = msBuildParams opts.MSBuildParams.Properties}
     }

// Declare build targets
Target.create "Clean" (fun _ ->  
  Fake.IO.Directory.delete publishDir
  Fake.IO.Directory.create publishDir
  Fake.IO.Directory.delete testResultsOutputDir
  Fake.IO.Directory.delete strykerOutputDir
)

Target.create "Restore" (fun _ ->    
  DotNet.restore id sln
)

Target.create "Build" (fun _ ->
  DotNet.build buildOptions sln 
)

Target.create "Test" (fun _ ->
  DotNet.test testOptions sln
)

Target.create "Package dotnet tool" (fun _ -> 
  DotNet.pack packOptions mainProj
)

Target.create "Publish apps" (fun _ ->
  [ "win-x64"; "linux-x64"; "linux-musl-x64" ] 
    |> List.map publishOptions
    |> List.iter (fun o -> DotNet.publish o mainProj)  
)

Target.create "Stryker" (fun _ ->  
  let opts (o: DotNet.Options) =
    { o with WorkingDirectory = testDir }
  DotNet.exec opts "dotnet-stryker" "" |> ignore
)

// Declare build dependencies
"Clean"
  ==> "Restore"
  ==> "Build"
  ==> "Test"
  ==> "Stryker"
  ==> "Package dotnet tool"
  ==> "Publish apps"
  
Target.runOrDefaultWithArguments  "Publish apps"