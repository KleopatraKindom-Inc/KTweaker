@echo off
setlocal

echo ===============================================
echo   KTweaker - Build Script
echo ===============================================
echo.

:: Always work from the folder this script lives in.
cd /d "%~dp0"

if not exist "KleopatraKindomTweaker.csproj" (
    echo [ERROR] KleopatraKindomTweaker.csproj not found next to build.bat.
    echo Put build.bat into the project folder and run it again.
    pause
    exit /b 1
)

:: --- Check .NET SDK is installed ---
where dotnet >nul 2>nul
if errorlevel 1 (
    echo [ERROR] .NET SDK not found in PATH.
    echo Download it from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

:: The project targets net8.0-windows, so the SDK must be version 8 or newer.
set SDK_MAJOR=0
for /f "tokens=1 delims=." %%v in ('dotnet --version') do set SDK_MAJOR=%%v
if %SDK_MAJOR% LSS 8 (
    echo [ERROR] .NET SDK 8 or newer is required - the project targets net8.0-windows.
    echo Run "dotnet --version" to see what is installed.
    echo Download SDK 8 from: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)

echo [1/3] .NET SDK found:
dotnet --version
echo.

:: Start from an empty output folder so files from older builds (for example the
:: pre-rename KleopatraKindomTweaker.exe) can never end up in the installer.
echo [2/3] Cleaning old output...
if exist publish rmdir /s /q publish
if exist publish (
    echo [ERROR] Could not clean .\publish - close Tweaker.exe if it is running and try again.
    pause
    exit /b 1
)
echo.

:: publish restores, builds and publishes in one go, so no separate restore/build steps.
:: No .pdb files: they only add size and are not needed to run the app.
echo [3/3] Publishing framework-dependent build to .\publish ...
echo       Small output - the target PC needs .NET 8 Desktop Runtime.
dotnet publish -c Release -r win-x64 --self-contained false -p:DebugType=none -p:DebugSymbols=false -o publish
if errorlevel 1 (
    echo [ERROR] Publish failed - see the messages above.
    pause
    exit /b 1
)

if not exist "publish\Tweaker.exe" (
    echo [ERROR] publish\Tweaker.exe was not produced. Check AssemblyName in the .csproj.
    pause
    exit /b 1
)

echo.
echo ===============================================
echo   Done!
echo   Output folder: .\publish
echo   Executable:    .\publish\Tweaker.exe
powershell -NoProfile -Command "$f = Get-Item 'publish\Tweaker.exe'; Write-Host ('  Version:       ' + $f.VersionInfo.FileVersion)"
powershell -NoProfile -Command "$s = (Get-ChildItem 'publish' -Recurse -File | Measure-Object Length -Sum).Sum; Write-Host ('  Total size:    {0:N1} MB' -f ($s / 1MB))"
echo.
echo   The app asks for administrator rights on start,
echo   because most tweaks write to protected registry keys.
echo ===============================================
pause
