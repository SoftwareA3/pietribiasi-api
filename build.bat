@echo off
setlocal

cd /d "%~dp0"

echo Cleanup previous distribution directory
if exist "%CD%\App" (
    rmdir /s /q "%CD%\App"
)

echo Setup Distribution Directory
mkdir "%CD%\App"

echo Publish ASP.NET Core Backend
dotnet publish .\apiPB\apiPB.csproj --configuration Release --output "%CD%\App\Backend" --runtime win-x64 --self-contained true /p:UseAppHost=true

echo Copy Frontend files in wwwroot
if exist "%CD%\App\Backend" ( 
    xcopy /s /y /i "%CD%\Frontend" "%CD%\App\Backend\wwwroot"
) else (
    echo Error: Backend directory not found.
    exit /b 1
)

echo Install dependencies
python -m pip install --upgrade pip
pip install pyinstaller

echo Build EXE
cd BuildScripts
pyinstaller --onefile --add-data "script_utils.py;." --add-data "build.json;." build.py
cd ..

echo Move EXE to distribution directory and cleanup
if exist "App/build.exe" (
    del build.exe
)
move "BuildScripts\dist\build.exe" "App\build.exe"
if exist "BuildScripts\dist" (
    rmdir /s /q "BuildScripts\dist"
)
if exist "BuildScripts\build" (
    rmdir /s /q "BuildScripts\build"
)
if exist "BuildScripts\build.spec" (
    del "BuildScripts\build.spec"
)

echo Move Documentation to distribution directory
if exist "%CD%\Docs\Documentazione.md" (
    copy "%CD%\Docs\Documentazione.md" "%CD%\App\Documentazione.md"
) else (
    echo Warning: Documentazione.md not found in Docs.
)

echo Move Configuration files to distribution directory
if exist "%CD%\BuildScripts\build.json" (
    copy "%CD%\BuildScripts\build.json" "%CD%\App\build.json"
) else (
    echo Warning: build.json not found in BuildScripts.
)

echo Build process completed.
endlocal
pause