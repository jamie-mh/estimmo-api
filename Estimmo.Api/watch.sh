#!/usr/bin/env bash

SCRIPT_DIR=$(dirname $(realpath -s "$0"))
CSPROJ_PATH=$(realpath -s "$SCRIPT_DIR/Estimmo.Api.csproj")

export DOTNET_ENVIRONMENT=Development
dotnet watch run --project "$CSPROJ_PATH"

