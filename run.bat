@echo off
setlocal

tasklist /FI "IMAGENAME eq apiPB.exe" | find /I "apiPB.exe" >nul
if not errorlevel 1 (
    echo A console "Pietribiasi App TestGround" is already active.
    echo.
    echo Copying Frontend files in wwwroot directory.

    xcopy /s /y /i "%CD%\Frontend\*" "%CD%\TestingGround\Backend\wwwroot"

    endlocal
    exit /b
)

echo Cleanup previous distribution directory
if exist "%CD%\TestingGround" (
    rmdir \s \q "%CD%\TestingGround"
)

echo Setup Distribution Directory
mkdir "%CD%\TestingGround"

echo Publish ASP.NET Core Backend
dotnet publish .\apiPB\apiPB.csproj --configuration Release --output "%CD%\TestingGround\Backend" --runtime win-x64 --self-contained true /p:UseAppHost=true

echo Copy Frontend files to compiled output (AFTER compilation)
xcopy /s /y /i "%CD%\Frontend\*" "%CD%\TestingGround\Backend\wwwroot"

echo Update appsettings.json with values from build.json
powershell -ExecutionPolicy Bypass -Command "$buildConfig = Get-Content 'BuildScripts\build.json' | ConvertFrom-Json; $appSettingsPath = 'TestingGround\Backend\appsettings.json'; if (Test-Path $appSettingsPath) { $appSettings = Get-Content $appSettingsPath | ConvertFrom-Json; if ($appSettings.Server -and $appSettings.Server.Backend) { $appSettings.Server.Backend.Host = $buildConfig.server.backend.host; } if ($appSettings.ConnectionStrings) { $appSettings.ConnectionStrings.LocalA3Db = $buildConfig.server.backend.connection_string; } $appSettings | ConvertTo-Json -Depth 10 | Set-Content $appSettingsPath; Write-Host 'appsettings.json updated successfully'; } else { Write-Host 'Warning: appsettings.json not found'; }"

echo Clean up: Remove wwwroot directory from source (optional)
if exist "%CD%\apiPB\wwwroot" (
    rmdir /s /q "%CD%\apiPB\wwwroot"
)

echo Executing apiPB.exe
if exist "%CD%\TestingGround\Backend" (
    cd "%CD%\TestingGround\Backend"
    if exist "apiPB.exe" (
        start "Pietribiasi App TestGround" apiPB.exe
        start "" "http://localhost:5245"
    )
) else (
    echo Warning: apiPB.exe not found in Backend directory.
)

endlocal