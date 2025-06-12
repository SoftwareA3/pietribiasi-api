@echo off
setlocal enabledelayedexpansion
title Controllo Applicazione

REM Variabili per memorizzare i PID dei processi
set BACKEND_PID=
set FRONTEND_PID=

REM Variabili di configurazione (valori di default)
set APP_NAME=Pietribiasi App
set BACKEND_PROJECT=apiPB
set FRONTEND_PORT=8080
set BACKEND_PORT=5245
set SERVER_IP=192.168.100.113

REM Carica la configurazione da build.json
call :LOAD_CONFIG_FROM_BUILD_JSON

:MENU
cls
echo ===============================================
echo    CONTROLLO APPLICAZIONE !APP_NAME!
echo ===============================================
echo.
echo 0. Avvia/Riavvia applicazione (apertura automatica su index.html)
echo 1. Avvia applicazione 
echo 2. Avvia solo il Frontend (apre la pagina index.html)
echo 3. Avvia solo il Backend
echo 4. Ferma applicazione  
echo 5. Riavvia applicazione
echo 6. Stato applicazione
echo 7. Mostra indirizzi IP
echo 8. Esci
echo.
set /p choice="Seleziona un'opzione (0-8): "

if "%choice%"=="0" goto FORCE_RESTART_WITH_INDEX
if "%choice%"=="1" goto START_APP
if "%choice%"=="2" goto START_FRONTEND_ONLY
if "%choice%"=="3" goto START_BACKEND_ONLY
if "%choice%"=="4" goto STOP_APP
if "%choice%"=="5" goto RESTART_APP
if "%choice%"=="6" goto STATUS_APP
if "%choice%"=="7" goto SHOW_IPS
if "%choice%"=="8" goto EXIT
goto MENU

:LOAD_CONFIG_FROM_BUILD_JSON
echo Caricamento configurazione da build.json...

REM Verifica se build.json esiste
if not exist "build.json" (
    echo ATTENZIONE: build.json non trovato, uso configurazione di default
    goto :eof
)

REM Estrae i valori da build.json usando PowerShell
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.app.name } catch { Write-Output 'Pietribiasi App' }"') do set APP_NAME=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.build.backend_project } catch { Write-Output 'apiPB' }"') do set BACKEND_PROJECT=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.backend.host } catch { Write-Output 'localhost' }"') do set SERVER_IP=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.backend.port } catch { Write-Output '5245' }"') do set BACKEND_PORT=%%i
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.server.frontend.port } catch { Write-Output '8080' }"') do set FRONTEND_PORT=%%i

echo Configurazione caricata:
echo   APP_NAME: !APP_NAME!
echo   BACKEND_PROJECT: !BACKEND_PROJECT!
echo   SERVER_IP: !SERVER_IP!
echo   BACKEND_PORT: !BACKEND_PORT!
echo   FRONTEND_PORT: !FRONTEND_PORT!
echo.

goto :eof

:FORCE_RESTART_WITH_INDEX
echo.
echo Riavvio forzato dell'applicazione con apertura automatica su index.html...
echo.
echo Arresto di eventuali processi in esecuzione...
call :STOP_APP_ENHANCED
timeout /t 2 >nul

echo Avvio del backend...
start "Backend Server" cmd /c "cd backend && !BACKEND_PROJECT!.exe --urls=http://!SERVER_IP!:!BACKEND_PORT!"
timeout /t 5 >nul

echo Avvio del frontend...
start "Frontend Server" cmd /c "npx http-server ./frontend -a !SERVER_IP! -p !FRONTEND_PORT! --cors -o index.html -c-1"
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
    echo Avvio del backend...
    start "Backend Server" cmd /c "cd backend && !BACKEND_PROJECT!.exe --urls=http://!SERVER_IP!:!BACKEND_PORT!"
    timeout /t 3 >nul
)

if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend...
    timeout /t 2 >nul
    start "Frontend Server" cmd /c "npx http-server ./frontend -a !SERVER_IP! -p !FRONTEND_PORT! --cors index.html -c-1"
)

echo.
echo Applicazione avviata!
call :SHOW_ADDRESSES
echo.
pause
goto MENU

:START_FRONTEND_ONLY
echo.
echo Avvio del solo Frontend...
echo.

call :CHECK_PROCESSES
if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend con apertura automatica su index.html...
    start "Frontend Server" cmd /c "npx http-server ./frontend -a !SERVER_IP! -p !FRONTEND_PORT! --cors -o index.html -c-1"
    timeout /t 3 >nul
    echo Frontend avviato!
)

echo.
echo Frontend disponibile su:
echo   http://localhost:!FRONTEND_PORT!
echo   http://!SERVER_IP!:!FRONTEND_PORT!
echo.
pause
goto MENU

:START_BACKEND_ONLY
echo.
echo Avvio del solo Backend...
echo.

call :CHECK_PROCESSES
if "!BACKEND_RUNNING!"=="1" (
    echo Backend già in esecuzione!
) else (
    echo Avvio del backend...
    start "Backend Server" cmd /c "cd backend && !BACKEND_PROJECT!.exe --urls=http://!SERVER_IP!:!BACKEND_PORT!"
    timeout /t 3 >nul
    echo Backend avviato!
)

echo.
echo Backend disponibile su:
echo   http://localhost:!BACKEND_PORT!
echo   http://!SERVER_IP!:!BACKEND_PORT!
echo.
pause
goto MENU

:STOP_APP
echo.
echo Arresto completo dell'applicazione...
echo.
call :STOP_APP_ENHANCED
echo.
echo Applicazione arrestata completamente!
echo.
pause
goto MENU

:RESTART_APP
echo.
echo Riavvio dell'applicazione...
call :STOP_APP_ENHANCED
timeout /t 2 >nul
goto START_APP

:STOP_APP_ENHANCED
REM Funzione migliorata per chiusura completa

echo Chiusura completa di processi e finestre console...

REM 1. Prima terminiamo i processi applicazione
echo Terminazione processi backend...
taskkill /f /im !BACKEND_PROJECT!.exe 2>nul

echo Terminazione processi frontend...
taskkill /f /im node.exe 2>nul

REM 2. Attendiamo che i processi terminino completamente
echo Attesa terminazione processi...
timeout /t 3 >nul

REM 3. Ora cerchiamo e chiudiamo le finestre CMD per comando di avvio
echo Chiusura finestre console backend...
wmic process where "name='cmd.exe' and commandline like '%%Backend Server%%'" delete 2>nul

echo Chiusura finestre console frontend...  
wmic process where "name='cmd.exe' and commandline like '%%Frontend Server%%'" delete 2>nul

REM 4. Metodo PowerShell per trovare finestre CMD con i nostri comandi
echo Pulizia finale finestre CMD...
powershell -Command "Get-WmiObject Win32_Process | Where-Object { $_.Name -eq 'cmd.exe' -and ($_.CommandLine -like '*cd backend*!BACKEND_PROJECT!.exe*' -or $_.CommandLine -like '*npx http-server*frontend*' -or $_.CommandLine -like '*Backend Server*' -or $_.CommandLine -like '*Frontend Server*') } | ForEach-Object { try { Write-Host \"Chiusura finestra CMD PID: $($_.ProcessId)\"; $_.Terminate() } catch {} }" 2>nul

REM 5. Metodo finale: cerca finestre che potrebbero essere tornate al titolo originale
timeout /t 1 >nul
taskkill /fi "WindowTitle eq Backend Server*" /f 2>nul
taskkill /fi "WindowTitle eq Frontend Server*" /f 2>nul

echo Pulizia completata.

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

REM Controlla se il backend è in esecuzione
tasklist | findstr "!BACKEND_PROJECT!.exe" >nul 2>&1
if %errorlevel%==0 set BACKEND_RUNNING=1

REM Controlla se http-server è in esecuzione
tasklist | findstr "node.exe" >nul 2>&1
if %errorlevel%==0 set FRONTEND_RUNNING=1

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
echo Backend:  http://localhost:!BACKEND_PORT!
echo Frontend: http://localhost:!FRONTEND_PORT!
echo.
echo Dall'esterno della rete:
echo Backend:  http://!SERVER_IP!:!BACKEND_PORT!
echo Frontend: http://!SERVER_IP!:!FRONTEND_PORT!
echo ----------------------------------------------
goto :eof

:EXIT
echo.
set /p confirm="Vuoi chiudere anche l'applicazione in esecuzione? (s/n): "
if /i "%confirm%"=="s" (
    echo.
    echo Chiusura completa dell'applicazione...
    call :STOP_APP_ENHANCED
    timeout /t 2 >nul
    echo Pulizia completata.
)
echo.
echo Arrivederci!
timeout /t 2 >nul
exit