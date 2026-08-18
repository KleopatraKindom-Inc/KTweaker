@echo off
setlocal enabledelayedexpansion

echo ===============================================
echo   Kleopatra Kindom Tweaker - Build Script
echo ===============================================
echo.

:: --- Check .NET SDK is installed ---
where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERROR] .NET SDK not found in PATH.
    echo Download it from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo [1/4] .NET SDK found:
dotnet --version
echo.

cd /d "%~dp0"

echo [2/4] Restoring NuGet packages...
dotnet restore
if errorlevel 1 (
    echo [ERROR] Restore failed.
    pause
    exit /b 1
)
echo.

echo [3/4] Building (Release)...
dotnet build -c Release
if errorlevel 1 (
    echo [ERROR] Build failed.
    pause
    exit /b 1
)
echo.

echo [4/4] Publishing self-contained single-file exe to .\publish ...
dotnet publish -c Release -r win-x64 --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -o publish
if errorlevel 1 (
    echo [ERROR] Publish failed.
    pause
    exit /b 1
)

echo.
echo ===============================================
echo   Done! Executable is in: .\publish\KleopatraKindomTweaker.exe
echo   (right-click -^> Run as administrator, since
echo    most tweaks need admin rights)
echo ===============================================
pause
