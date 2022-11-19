[CmdletBinding()]
param(
    [Parameter()]
    [switch]
    $IncludePrerelease
)

function Get-LatestCarterPackageVersion {
    [CmdletBinding()]
    param(
        [Parameter()]
        [switch]
        $IncludePrerelease
    )

    #
    # NuGet Server API docs
    # https://learn.microsoft.com/en-us/nuget/api/overview
    #

    $nugetApiServiceIndexUrl = 'https://api.nuget.org/v3/index.json'
    $serviceIndexResponse = Invoke-RestMethod -Method Get -Uri $nugetApiServiceIndexUrl

    $nugetApiSearchQueryServiceUrl = $serviceIndexResponse.resources |
        Where-Object '@type' -eq 'SearchQueryService' |
        Select-Object -First 1 |
        Select-Object -ExpandProperty '@id'

    #
    # https://learn.microsoft.com/en-us/nuget/api/search-query-service-resource#search-for-packages
    #
    $queryParameters = @{
        q           = 'Carter'
        prerelease  = if ($IncludePrerelease) { 'true' } else { 'false' }
        semVerLevel = '2.0.0'
    }

    $searchResponse = Invoke-RestMethod `
        -Method Get `
        -Uri $nugetApiSearchQueryServiceUrl `
        -Body $queryParameters

    $latestPackage = $searchResponse.data |
        Where-Object 'id' -eq 'Carter' |
        Sort-Object -Property 'version' -Descending |
        Select-Object -First 1

    Write-Output -InputObject $latestPackage.version
}

function Update-CarterPackageReferences {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]
        $RootPath,

        [Parameter(Mandatory = $true)]
        [string]
        $PackageVersion
    )

    $projectFiles = Get-ChildItem `
        -Path $RootPath `
        -Recurse `
        -File `
        -Filter '*.csproj'

    $regexPattern = '(?<beforeVersion><PackageReference\s+Include="Carter"\s+Version=")[\s\S]+?(?<afterVersion>"\s*/>)'

    foreach ($projectFile in $projectFiles) {
        $contents = Get-Content -Path $projectFile.FullName -Raw -Encoding utf8

        if ([System.Text.RegularExpressions.Regex]::IsMatch($contents, $regexPattern)) {
            $newContents = [System.Text.RegularExpressions.Regex]::Replace(
                $contents,
                $regexPattern,
                ('${{beforeVersion}}{0}${{afterVersion}}' -f $PackageVersion))

            Set-Content -Path $projectFile.FullName -Value $newContents -Encoding utf8 -NoNewline
        }
    }
}

$targetCarterPackageVersion = Get-LatestCarterPackageVersion -IncludePrerelease:$IncludePrerelease
Update-CarterPackageReferences -RootPath $PSScriptRoot -PackageVersion $targetCarterPackageVersion
