param (
    [Parameter(Mandatory=$false)]
    [string]$Target = ""
)

$ErrorActionPreference = "Stop"

& dotnet tool restore

if($Target -eq "") {
    & dotnet run --project build.fsproj -- -t "BuildTestAndPackage"
} else {
    & dotnet run --project build.fsproj -- -t $Target
}
