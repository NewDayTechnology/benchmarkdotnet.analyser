![Build](https://github.com/NewDayTechnology/benchmarkdotnet.analyser/actions/workflows/actions_buildtestpackage.yml/badge.svg)
 [![Contributor Covenant](https://img.shields.io/badge/Contributor%20Covenant-2.0-4baaaa.svg)](code_of_conduct.md)

# BenchmarkDotNet Analyser

## Description
A CLI for analysing [BenchmarkDotNet](https://benchmarkdotnet.org/) result data.![Front](./docs/cli_front.png)

BDNA collects, aggregates and analyses [BenchmarkDotNet](https://benchmarkdotnet.org/) results for performance degredations. If you have benchmarks running on critical code, and want to ensure all your builds have acceptable performance, BDNA might help you.

---

### How BDNA collects data

BDNA collects [BenchmarkDotNet](https://benchmarkdotnet.org/) result files. These files are usually found under ``./BenchmarkDotNet.Artifacts/results`` folders. BDNA will aggregate these results folders into a single dataset (in reality a local disk folder) and analyse them for performance degradation. 

To aggregate, use ``bdna aggregate``, giving the folder containing new results, the folder containing a previous aggregated dataset, and the folder for the new dataset. If you want to retain the same dataset folder feel free. Folders containing aggregated datasets may be zipped and fed into future runs of ``bdna aggregate`` in your CI pipeline.

Incidentally, if you're familiar with FP's ``fold`` pattern, you'll immediately recognise this aggregation style.

### How BDNA compares benchmarks
Every benchmark run (benchmark type, method and parameter) gives a set of values over repeated runs: think of these as simple 1-dimensional time series data. BDNA will analyse only these specific runs. 

For example, if your benchmarks have runs ``Dijkstra(Depth=1)`` & ``Dijkstra(Depth=2)`` each of these will be analysed independently, and not ``Dijkstra()`` as a whole.

For each run, BDNA uses the minimum mean value per run as the baseline value. It chooses the the latest result (what you've just added) as the test value: if that test value is within your tolerances, the analysis will pass, if not the analysis fails. 

Use ``bdna analyse`` to scan the dataset.

---

### How this works within CI

#### Persisting datasets 
BDNA has no integration with an external data store such as a database or blob storage provider. All its persistence is through plain old local storage. Many CI pipelines provide RESTful APIs to access prior builds' artifacts. Simply configure your CI build job to retain the aggregated datasets within your CI build artifacts, and use its API to download a previous build's artifacts. This way you'll shouldn't need any expensive databases.

#### Failing build jobs in CI
A failed ``bdna analysis`` run will result in a non-zero command line return code. Most CI platforms will fail on non-zero return codes and it is this return value you should use to stop builds on degraded performance.

BDNA will send details of failed benchmarks to the console and thus to your build log if you want more details.

---

## Technologies used
We use C# and .NET Core 5.0 for this project.

Some 3rd party packages that we depend on:-
* [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
* [Nate McMaster's Command line utils](https://www.nuget.org/packages/McMaster.Extensions.CommandLineUtils)
* [Microsoft's Dependency Injection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection)
* [Crayon](https://www.nuget.org/packages/Crayon/)
* [FluentAssertions](https://www.nuget.org/packages/FluentAssertions)
* [FsCheck](https://www.nuget.org/packages/FsCheck.Xunit)
* [NSubstitute](https://www.nuget.org/packages/NSubstitute/)
* [Coverlet](https://www.nuget.org/packages/coverlet.collector/)
* [Xunit](https://www.nuget.org/packages/xunit/)
* [Stryker](https://stryker-mutator.io/docs/stryker-net/Introduction/)
* [FAKE](https://fake.build/)

---

## Building 

The local build script is ``build.ps1``. As it integrates with [FAKE](https://fake.build/), build target selection is available. 

``.\build.ps1`` for the full build workflow.

``.\build.ps1 Build`` to only build the repository, without test nor verification.

``.\build.ps1 Test`` to only build and test

---

## More reading

[License](LICENSE)

[Copyright notice](NOTICE)

[How to Contribute](CONTRIBUTING.md)

[Our Code of Conduct](CODE_OF_CONDUCT.md)

[Security notes](SECURITY.md)

[How we support this project](SUPPORT.md)
