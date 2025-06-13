@echo off
setlocal enabledelayedexpansion
setlocal enableextensions  

set HOST=localhost
set PORT=0000

for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.frontend.host } catch { Write-Output 'localhost' }"') do set HOST=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.frontend.port } catch { Write-Output '8080' }"') do set PORT=%%i

echo Configurazione caricata:
echo   HOST: !HOST!
echo   PORT: !PORT!

:: Vai nella cartella del frontend
:: cd /d "%~dp0Frontend"

:: Controlla se http-server è installato
where http-server >nul 2>&1
IF %ERRORLEVEL% NEQ 0 (
    echo http-server non trovato. Installazione in corso...
    npm install -g http-server
)

:: Avvia http-server sulla porta letta da JSON
echo Avvio del frontend con apertura automatica su index.html...
:: npx http-server ./Frontend -a !HOST! -p !PORT! --cors -o index.html -c-1
python python_server.py
timeout /t 3 >nul
echo Frontend avviato!