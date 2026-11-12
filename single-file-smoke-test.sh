#!/usr/bin/env bash
set -euo pipefail

# Publishes the CarterSample app as a single file, both framework-dependent and self-contained,
# and checks that modules and validators are still discovered.

PORT=5099
BASE_URL="http://127.0.0.1:$PORT"
OUTPUT_ROOT="$(mktemp -d)"
APP_PID=""

cleanup() {
  if [ -n "$APP_PID" ]; then
    kill "$APP_PID" 2>/dev/null || true
    wait "$APP_PID" 2>/dev/null || true
  fi
  rm -rf "$OUTPUT_ROOT"
}
trap cleanup EXIT

expect_status() {
  local description=$1
  local expected=$2
  shift 2

  local actual
  actual="$(curl --silent --output /dev/null --write-out "%{http_code}" "$@")"

  if [ "$actual" != "$expected" ]; then
    echo "FAILED: $description returned $actual, expected $expected"
    cat "$OUTPUT_DIR/app.log"
    exit 1
  fi

  echo "OK: $description returned $actual"
}

for SELF_CONTAINED in false true; do
  echo "Single file publish, self-contained=$SELF_CONTAINED"
  OUTPUT_DIR="$OUTPUT_ROOT/self-contained-$SELF_CONTAINED"

  dotnet publish ./samples/CarterSample/CarterSample.csproj \
    --configuration Release \
    --use-current-runtime \
    --self-contained "$SELF_CONTAINED" \
    -p:PublishSingleFile=true \
    --output "$OUTPUT_DIR" \
    --nologo

  (cd "$OUTPUT_DIR" && exec ./CarterSample --urls "$BASE_URL" > app.log 2>&1) &
  APP_PID=$!

  for _ in $(seq 1 30); do
    if ! kill -0 "$APP_PID" 2>/dev/null; then
      echo "FAILED: the app exited during startup"
      cat "$OUTPUT_DIR/app.log"
      exit 1
    fi
    if curl --silent --output /dev/null "$BASE_URL"; then
      break
    fi
    sleep 1
  done

  # A module in the entry assembly
  expect_status "GET /actors" 200 "$BASE_URL/actors"

  # A validator in a referenced assembly that is not loaded when AddCarter runs
  expect_status "POST /castmembers with an invalid body" 422 \
    --request POST --header "Content-Type: application/json" --data '{"name":""}' "$BASE_URL/castmembers"

  kill "$APP_PID"
  wait "$APP_PID" 2>/dev/null || true
  APP_PID=""
done

echo "Single file smoke test passed"
