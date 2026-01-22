@echo off
chcp 65001 >nul
dotnet publish ./catalog-convert --configuration Release --framework net10.0 -o ./build/win-64 -r win-x64 --self-contained true /p:PublishTrimmed=true
dotnet publish ./catalog-convert --configuration Release --framework net10.0 -o ./build/linux-x64 -r linux-x64 --self-contained true /p:PublishTrimmed=true
dotnet publish ./catalog-convert --configuration Release --framework net10.0 -o ./build/linux-arm64 -r linux-arm64 --self-contained true /p:PublishTrimmed=true
