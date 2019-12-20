#!/usr/bin/env bash
set -euo pipefail

for package in $(find -name "*.nupkg" | grep "test" -v); do
  echo "${0##*/}": Pushing $package...
  dotnet nuget push $package --source https://www.myget.org/F/adamralph-ci/api/v2/package --api-key $1
done
