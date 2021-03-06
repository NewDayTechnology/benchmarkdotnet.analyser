param (
    [Parameter(Mandatory=$false)]
    [string]$Target = ""
)

$ErrorActionPreference = "Stop"

& dotnet tool restore

if($Target -eq "") {
    & dotnet fake run FakeBuild.fsx
} else {
    & dotnet fake run FakeBuild.fsx --target $Target
}
