@echo off
setlocal enabledelayedexpansion
title Controllo Applicazione

REM Variabili per memorizzare i PID dei processi
set BACKEND_PID=
set FRONTEND_PID=

:MENU
cls
echo ===============================================
echo    CONTROLLO APPLICAZIONE PIETRIBIASI_API
echo ===============================================
echo.
echo 1. Avvia applicazione
echo 2. Ferma applicazione  
echo 3. Riavvia applicazione
echo 4. Stato applicazione
echo 5. Mostra indirizzi IP
echo 6. Esci
echo.
set /p choice="Seleziona un'opzione (1-6): "

if "%choice%"=="1" goto START_APP
if "%choice%"=="2" goto STOP_APP
if "%choice%"=="3" goto RESTART_APP
if "%choice%"=="4" goto STATUS_APP
if "%choice%"=="5" goto SHOW_IPS
if "%choice%"=="6" goto EXIT
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
    start "Backend Server" cmd /k "cd apiPB && dotnet run"
    timeout /t 3 >nul
)

if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend...
    timeout /t 2 >nul
    start "Frontend Server" cmd /k "cd Frontend && http-server -a 0.0.0.0 -p 8080 --cors index.html -c-1"
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
echo Backend:  http://localhost:5245
echo Frontend: http://localhost:8080
echo.
echo Dall'esterno della rete:
echo Backend:  http://192.168.100.113:5245
echo Frontend: http://192.168.100.113:8080
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