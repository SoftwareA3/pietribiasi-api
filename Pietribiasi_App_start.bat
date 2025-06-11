@echo off
setlocal enabledelayedexpansion
title Controllo Applicazione

REM Variabili per memorizzare i PID dei processi
set BACKEND_PID=
set FRONTEND_PID=

REM Carica la configurazione da build.json
call :LOAD_CONFIG_FROM_BUILD_JSON

:MENU
cls
echo ===============================================
echo    CONTROLLO APPLICAZIONE PIETRIBIASI_API
echo ===============================================
echo.
echo 0. Avvia/Riavvia applicazione (apertura automatica su index.html)
echo 1. Avvia applicazione 
echo 2. Ferma applicazione  
echo 3. Riavvia applicazione
echo 4. Stato applicazione
echo 5. Mostra indirizzi IP
echo 6. Esci
echo.
set /p choice="Seleziona un'opzione (0-6): "

if "%choice%"=="0" goto FORCE_RESTART_WITH_INDEX
if "%choice%"=="1" goto START_APP
if "%choice%"=="2" goto STOP_APP
if "%choice%"=="3" goto RESTART_APP
if "%choice%"=="4" goto STATUS_APP
if "%choice%"=="5" goto SHOW_IPS
if "%choice%"=="6" goto EXIT
goto MENU

:LOAD_CONFIG_FROM_BUILD_JSON
echo Caricamento configurazione da build.json...

REM Valori di default nel caso build.json non sia disponibile
set FRONTEND_PORT=8080
set BACKEND_PORT=5245  
set SERVER_IP=192.168.100.113

REM Verifica se build.json esiste
if not exist "build.json" (
    echo ATTENZIONE: build.json non trovato, uso configurazione di default
    goto :eof
)

REM Estrae i valori da build.json usando PowerShell
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.backend.host } catch { Write-Output '192.168.100.113' }"') do set SERVER_IP=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.backend.port } catch { Write-Output '5245' }"') do set BACKEND_PORT=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.frontend.port } catch { Write-Output '8080' }"') do set FRONTEND_PORT=%%i

echo Configurazione caricata:
echo   SERVER_IP: %SERVER_IP%
echo   BACKEND_PORT: %BACKEND_PORT%
echo   FRONTEND_PORT: %FRONTEND_PORT%
echo.

goto :eof

:FORCE_RESTART_WITH_INDEX
echo.
echo Riavvio forzato dell'applicazione con apertura automatica su index.html...
echo.
echo Arresto di eventuali processi in esecuzione...
call :STOP_APP_SILENT
timeout /t 2 >nul

echo Configurazione dell'API URL...
call :UPDATE_API_CONFIG

echo Aggiornamento appsettings.json...
call :UPDATE_APPSETTINGS

echo Avvio del backend...
start "Backend Server" cmd /k "cd apiPB && dotnet run"
timeout /t 5 >nul

echo Avvio del frontend...
start "Frontend Server" cmd /k "cd Frontend && http-server -a 0.0.0.0 -p %FRONTEND_PORT% --cors -o index.html -c-1"
timeout /t 3 >nul

echo.
echo Applicazione riavviata! Apertura automatica della pagina...
call :SHOW_ADDRESSES
echo.
pause
goto MENU

:START_APP
echo.
echo Avvio dell'applicazione...
echo.

REM Controlla se i processi sono già in esecuzione
call :CHECK_PROCESSES
if "!BACKEND_RUNNING!"=="1" (
    echo Backend già in esecuzione!
) else (
    echo Configurazione dell'API URL...
    call :UPDATE_API_CONFIG
    
    echo Aggiornamento appsettings.json...
    call :UPDATE_APPSETTINGS
    
    echo Avvio del backend...
    start "Backend Server" cmd /k "cd apiPB && dotnet run"
    timeout /t 3 >nul
)

if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend...
    timeout /t 2 >nul
    start "Frontend Server" cmd /k "cd Frontend && http-server -a 0.0.0.0 -p %FRONTEND_PORT% --cors index.html -c-1"
)

echo.
echo Applicazione avviata!
call :SHOW_ADDRESSES
echo.
pause
goto MENU

:STOP_APP
echo.
echo Arresto dell'applicazione...
echo.

REM Termina i processi dotnet
echo Chiusura backend...
taskkill /f /im dotnet.exe 2>nul
if %errorlevel%==0 (
    echo Backend chiuso con successo.
) else (
    echo Nessun processo backend da chiudere.
)

REM Termina i processi http-server
echo Chiusura frontend...
taskkill /f /im node.exe 2>nul
if %errorlevel%==0 (
    echo Frontend chiuso con successo.
) else (
    echo Nessun processo frontend da chiudere.
)

REM Chiudi le finestre cmd specifiche
taskkill /fi "WindowTitle eq Backend Server*" /f 2>nul
taskkill /fi "WindowTitle eq Frontend Server*" /f 2>nul

echo.
echo Applicazione arrestata!
echo.
pause
goto MENU

:RESTART_APP
echo.
echo Riavvio dell'applicazione...
call :STOP_APP_SILENT
timeout /t 2 >nul
goto START_APP

:STOP_APP_SILENT
taskkill /f /im dotnet.exe 2>nul
taskkill /f /im node.exe 2>nul
taskkill /fi "WindowTitle eq Backend Server*" /f 2>nul
taskkill /fi "WindowTitle eq Frontend Server*" /f 2>nul
goto :eof

:STATUS_APP
echo.
echo Stato dell'applicazione:
echo.

call :CHECK_PROCESSES

if "!BACKEND_RUNNING!"=="1" (
    echo [ATTIVO] Backend Server
) else (
    echo [INATTIVO] Backend Server
)

if "!FRONTEND_RUNNING!"=="1" (
    echo [ATTIVO] Frontend Server
) else (
    echo [INATTIVO] Frontend Server
)

echo.
pause
goto MENU

:CHECK_PROCESSES
set BACKEND_RUNNING=0
set FRONTEND_RUNNING=0

REM Controlla se dotnet è in esecuzione
tasklist | findstr "dotnet.exe" >nul 2>&1
if %errorlevel%==0 set BACKEND_RUNNING=1

REM Controlla se http-server è in esecuzione
tasklist | findstr "node.exe" >nul 2>&1
if %errorlevel%==0 set FRONTEND_RUNNING=1

goto :eof

:UPDATE_APPSETTINGS
echo Aggiornamento appsettings.json con configurazione da build.json...

set APPSETTINGS_PATH=apiPB\appsettings.json

if not exist "%APPSETTINGS_PATH%" (
    echo ERRORE: File appsettings.json non trovato in %APPSETTINGS_PATH%
    goto :eof
)

REM Crea uno script PowerShell temporaneo
set PS_SCRIPT=%TEMP%\update_appsettings.ps1

echo try { > "%PS_SCRIPT%"
echo     $appsettings = Get-Content '%APPSETTINGS_PATH%' ^| ConvertFrom-Json >> "%PS_SCRIPT%"
echo     if (-not $appsettings.Server) { $appsettings ^| Add-Member -MemberType NoteProperty -Name 'Server' -Value @{} } >> "%PS_SCRIPT%"
echo     if (-not $appsettings.Server.Backend) { $appsettings.Server ^| Add-Member -MemberType NoteProperty -Name 'Backend' -Value @{} } >> "%PS_SCRIPT%"
echo     if (-not $appsettings.Server.Frontend) { $appsettings.Server ^| Add-Member -MemberType NoteProperty -Name 'Frontend' -Value @{} } >> "%PS_SCRIPT%"
echo     $appsettings.Server.Backend.Host = '%SERVER_IP%' >> "%PS_SCRIPT%"
echo     $appsettings.Server.Backend.Port = [int]'%BACKEND_PORT%' >> "%PS_SCRIPT%"
echo     $appsettings.Server.Frontend.Host = '%SERVER_IP%' >> "%PS_SCRIPT%"
echo     $appsettings.Server.Frontend.Port = [int]'%FRONTEND_PORT%' >> "%PS_SCRIPT%"
echo     $appsettings ^| ConvertTo-Json -Depth 10 ^| Set-Content '%APPSETTINGS_PATH%' -Encoding UTF8 >> "%PS_SCRIPT%"
echo     Write-Output 'appsettings.json aggiornato con successo' >> "%PS_SCRIPT%"
echo } catch { >> "%PS_SCRIPT%"
echo     Write-Output 'Errore durante aggiornamento appsettings.json: ' + $_.Exception.Message >> "%PS_SCRIPT%"
echo } >> "%PS_SCRIPT%"

REM Esegui lo script PowerShell
powershell -ExecutionPolicy Bypass -File "%PS_SCRIPT%"

REM Pulisci il file temporaneo
del "%PS_SCRIPT%" 2>nul

goto :eof

:UPDATE_API_CONFIG
echo Aggiornamento configurazione API...

REM Percorso del file main.js
set MAIN_JS_PATH=Frontend\Web\javascript\main.js

REM Verifica che il file esista
if not exist "%MAIN_JS_PATH%" (
    echo ERRORE: File main.js non trovato in %MAIN_JS_PATH%
    pause
    goto MENU
)

REM Crea una copia di backup
copy "%MAIN_JS_PATH%" "%MAIN_JS_PATH%.bak" >nul 2>&1

REM URL del backend
set BACKEND_URL=http://%SERVER_IP%:%BACKEND_PORT%

REM Crea un file temporaneo per la sostituzione
set TEMP_FILE=%TEMP%\main_js_temp.js

REM Sostituisce il placeholder con l'URL effettivo del backend
powershell -Command "(Get-Content '%MAIN_JS_PATH%') -replace '##API_BASE_URL##', '%BACKEND_URL%' | Set-Content '%TEMP_FILE%'"

REM Verifica se la sostituzione è andata a buon fine
if exist "%TEMP_FILE%" (
    move "%TEMP_FILE%" "%MAIN_JS_PATH%" >nul
    echo API URL configurato: %BACKEND_URL%
) else (
    echo ERRORE: Impossibile aggiornare la configurazione API
    pause
)

goto :eof

:SHOW_IPS
call :SHOW_ADDRESSES
pause
goto MENU

:SHOW_ADDRESSES
echo.
echo ----------------------------------------------
echo Per accedere all'applicazione:
echo.
echo Indirizzi IP disponibili:
ipconfig | findstr IPv4
echo.
echo Backend:  http://localhost:%BACKEND_PORT%
echo Frontend: http://localhost:%FRONTEND_PORT%
echo.
echo Dall'esterno della rete:
echo Backend:  http://%SERVER_IP%:%BACKEND_PORT%
echo Frontend: http://%SERVER_IP%:%FRONTEND_PORT%
echo ----------------------------------------------
goto :eof

:EXIT
echo.
set /p confirm="Vuoi chiudere anche l'applicazione in esecuzione? (s/n): "
if /i "%confirm%"=="s" (
    call :STOP_APP_SILENT
    echo Applicazione chiusa.
)
echo.
echo Arrivederci!
timeout /t 2 >nul
exit