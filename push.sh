#!/usr/bin/env bash
set -euo pipefail

TARGET_PACKAGE=$1
NUGET_TARGET_SERVICE=$2
NUGET_API_KEY=$3

if [ "$TARGET_PACKAGE" = "carter" ]; then
  # Publish both the Carter and the CarterTemplate packages
  TARGET_PACKAGES="$(find -wholename "./src/Carter/**/*.nupkg" -or -wholename "./template/**/*.nupkg" -or -wholename "./src/Carter.Analyzers/**/*.nupkg")"
elif [ "$TARGET_PACKAGE" = "newtonsoft" ]; then
  TARGET_PACKAGES="$(find -wholename "./src/Carter.ResponseNegotiators.Newtonsoft/**/*.nupkg")"
else
  echo "Unexpected target package name \"$TARGET_PACKAGE\"; Accepted values: carter, newtonsoft"
  exit 1
fi

if [ "$NUGET_TARGET_SERVICE" = "feedz" ]; then
  NUGET_TARGET_URL="https://f.feedz.io/carter/carter/nuget/index.json"
elif [ "$NUGET_TARGET_SERVICE" = "nuget" ]; then
  NUGET_TARGET_URL="https://api.nuget.org/v3/index.json"
else
  echo "Unexpected NuGet service name \"$NUGET_TARGET_SERVICE\"; Accepted values: feedz, nuget"
  exit 1
fi

for package in $(echo "$TARGET_PACKAGES" | grep "test" -v); do
  echo "${0##*/}": Pushing $package to $NUGET_TARGET_SERVICE \($NUGET_TARGET_URL\)...
  dotnet nuget push $package --source $NUGET_TARGET_URL --api-key $NUGET_API_KEY
done
