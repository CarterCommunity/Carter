<#
.SYNOPSIS
    Replaces the Carter project reference by a NuGet package reference in a .csproj file
.EXAMPLE
    ./replace-project-reference-with-package-reference.ps1 -ProjectFile ./template/content/CarterTemplate.csproj -PackageVersion '7.0.0'
#>
[CmdletBinding()]
param (
    # The path to the .csproj file that needs to be updated
    [Parameter(Mandatory = $true)]
    [string]
    $ProjectFile,

    # The version of the Carter package to use in the package reference element
    [Parameter(Mandatory = $true)]
    [string]
    $PackageVersion
)

if (-not (Test-Path -Path $ProjectFile)) {
    throw ('The project file ''{0}'' does not exist' -f $ProjectFile)
}

$projectFileContents = Get-Content -Path $ProjectFile -Raw -Encoding utf8
$newProjectFileContents = $projectFileContents -replace '<ProjectReference [\s\S]+?/>', ('<PackageReference Include="Carter" Version="{0}" />' -f $PackageVersion)

Set-Content -Path $ProjectFile -Value $newProjectFileContents -Encoding utf8
