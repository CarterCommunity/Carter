#!/usr/bin/env bash
set -euo pipefail

TARGET_PACKAGE=$1
if [ "$TARGET_PACKAGE" = "carter" ]; then
  TARGET_PACKAGE_PATH="./src/Carter/**/*.nupkg"
elif [ "$TARGET_PACKAGE" = "newtonsoft" ]; then
  TARGET_PACKAGE_PATH="./src/Carter.ResponseNegotiators.Newtonsoft/**/*.nupkg"
fi

NUGET_TARGET_SERVICE=$2
if [ "$NUGET_TARGET_SERVICE" = "feedz" ]; then
  NUGET_TARGET_URL="https://f.feedz.io/carter/carter/nuget/index.json"
elif [ "$NUGET_TARGET_SERVICE" = "nuget" ]; then
  NUGET_TARGET_URL="https://somenugeturl/index.json"
fi

for package in $(find -wholename "$TARGET_PACKAGE_PATH" | grep "test" -v); do
  echo "${0##*/}": Pushing $package to $NUGET_TARGET_SERVICE \($NUGET_TARGET_URL\)...
  # dotnet nuget push $package --source https://f.feedz.io/carter/carter/nuget/index.json --api-key $1
done
