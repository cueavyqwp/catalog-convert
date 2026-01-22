@echo off
chcp 65001 >nul
dotnet publish ./catalog-convert --configuration Release --framework net10.0 -o ./build
