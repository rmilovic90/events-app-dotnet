#!/bin/bash

echo "Starting PostgreSQL DB conatiner ..."

docker compose -f ./src/WebApi/compose.yml up -d

echo "Starting application ..."

dotnet run --project ./src/WebApi/WebApi.csproj