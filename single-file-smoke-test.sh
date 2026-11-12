#!/usr/bin/env bash
set -euo pipefail

# Publishes samples/SingleFileSample as a single file and checks that Carter
# still finds its module and validators. Runs once as a framework-dependent
# app and once as a self-contained app.

PROJECT="./samples/SingleFileSample/SingleFileSample.csproj"
URL="http://127.0.0.1:5099"
PUBLISH_DIR="$(mktemp -d)"
APP_PID=""

publish_app() {
  local self_contained=$1

  dotnet publish "$PROJECT" \
    --configuration Release \
    --self-contained "$self_contained" \
    --output "$PUBLISH_DIR" \
    --nologo \
    -p:GeneratePackageOnBuild=false # don't overwrite the NuGet packages created by the main build
}

start_app() {
  "$PUBLISH_DIR/SingleFileSample" --urls "$URL" > "$PUBLISH_DIR/app.log" 2>&1 &
  APP_PID=$!

  # Wait up to 30 seconds for the app to accept requests
  for _ in $(seq 1 30); do
    if curl --silent --output /dev/null "$URL"; then
      return
    fi
    sleep 1
  done

  fail "the app did not start"
}

stop_app() {
  if [ -n "$APP_PID" ]; then
    kill "$APP_PID" 2>/dev/null || true
    wait "$APP_PID" 2>/dev/null || true
    APP_PID=""
  fi
}

fail() {
  echo "FAILED: $1"
  cat "$PUBLISH_DIR/app.log"
  exit 1
}

# Usage: expect_status <expected status> <description> <curl arguments>
expect_status() {
  local expected=$1
  local description=$2
  shift 2

  local actual
  actual="$(curl --silent --output /dev/null --write-out "%{http_code}" "$@")"

  if [ "$actual" != "$expected" ]; then
    fail "$description returned $actual, expected $expected"
  fi

  echo "OK: $description returned $actual"
}

post_empty_name() {
  local path=$1

  expect_status 422 "POST $path with an empty name" \
    --request POST \
    --header "Content-Type: application/json" \
    --data '{"name":""}' \
    "$URL$path"
}

cleanup() {
  stop_app
  rm -rf "$PUBLISH_DIR"
}
trap cleanup EXIT

for self_contained in false true; do
  echo "Single file publish, self-contained=$self_contained"

  publish_app "$self_contained"
  start_app

  # The module is found
  expect_status 200 "GET /" "$URL/"

  # The validator in the app's own assembly is found
  post_empty_name "/greetings"

  # The validator in the referenced ValidatorOnlyProject assembly is found
  post_empty_name "/castmembers"

  stop_app
done

echo "Single file smoke test passed"
