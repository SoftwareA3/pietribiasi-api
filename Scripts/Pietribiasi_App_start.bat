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
echo 0. Avvia applicazione in modalità silenziosa (senza console)
echo 1. Avvia applicazione 
echo 2. Avvia solo il Backend
echo 3. Ferma applicazione  
echo 4. Riavvia applicazione
echo 5. Stato applicazione
echo 6. Mostra indirizzi IP
echo 7. Esci
echo.
set /p choice="Seleziona un'opzione (0-7): "

if "%choice%"=="0" goto START_APP_SILENT
if "%choice%"=="1" goto START_APP
if "%choice%"=="2" goto START_BACKEND_ONLY
if "%choice%"=="3" goto STOP_APP
if "%choice%"=="4" goto RESTART_APP
if "%choice%"=="5" goto STATUS_APP
if "%choice%"=="6" goto SHOW_IPS
if "%choice%"=="7" goto EXIT
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
for /f "delims=" %%i in ('powershell -Command "try { $json = Get-Content 'build.json' | ConvertFrom-Json; Write-Output $json.remote_backend.host } catch { Write-Output 'localhost' }"') do set SERVER_IP=%%i
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

:START_APP_SILENT
echo.
echo Avvio Applicazione senza console...
echo.

REM Controlla se i processi sono già in esecuzione
call :CHECK_PROCESSES
if "!BACKEND_RUNNING!"=="1" (
    echo Backend già in esecuzione!
) else (
    echo Avvio del backend...
    REM Avvia il backend minimizzato e poi nasconde la finestra della console
    start "Backend Server" /min cmd /c "cd backend && !BACKEND_PROJECT!.exe --urls=http://!SERVER_IP!:!BACKEND_PORT!"
    REM Attendi che la finestra venga creata
    timeout /t 1 >nul
    REM Nascondi la finestra della console del backend usando PowerShell
    powershell -Command "Get-Process | Where-Object { $_.MainWindowTitle -like '*Backend Server*' } | ForEach-Object { $hwnd = $_.MainWindowHandle; if ($hwnd -ne 0) { Add-Type -Name win -Namespace native -MemberDefinition '[DllImport(\"user32.dll\")]public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);'; [native.win]::ShowWindowAsync($hwnd, 0) } }"
    timeout /t 3 >nul
)

if "!FRONTEND_RUNNING!"=="1" (
    echo Frontend già in esecuzione!
) else (
    echo Avvio del frontend...
    timeout /t 2 >nul
    REM Avvia il server frontend 
    start "Pietribiasi Frontend Server" pythonw server_only.py
)

echo.
echo Applicazione avviata!
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
    start "Pietribiasi Frontend Server" python server_only.py
)

echo.
echo Applicazione avviata!
call :SHOW_ADDRESSES
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

echo Chiusura completa di processi e finestre...

REM 1. Termina i processi Python (python_server.py)
echo Terminazione processi frontend Python...
taskkill /f /im python.exe 2>nul
taskkill /f /im pythonw.exe 2>nul

REM 2. Termina i processi backend
echo Terminazione processi backend...
taskkill /f /im !BACKEND_PROJECT!.exe 2>nul

REM 3. Termina eventuali processi WebView (nel caso il frontend usi webview) e il progesso PietribiasiApp.exe
REM (assicurati che il nome del processo sia corretto)
echo Terminazione processi WebView...
taskkill /f /im "Microsoft Edge WebView2" 2>nul
taskkill /f /im msedgewebview2.exe 2>nul
echo Terminazione processi PietribiasiApp...
tasklist | findstr PietribiasiApp.exe >nul 2>&1
if %errorlevel%==0 (
    taskkill /f /im PietribiasiApp.exe 2>nul
) else (
    echo Nessun processo PietribiasiApp trovato.
)

REM 4. Attendiamo che i processi terminino completamente
echo Attesa terminazione processi...
timeout /t 3 >nul

REM 5. Chiusura finestre console specifiche
echo Chiusura finestre console backend...
wmic process where "name='cmd.exe' and commandline like '%%Backend Server%%'" delete 2>nul

echo Chiusura finestre console frontend...  
wmic process where "name='cmd.exe' and commandline like '%%Pietribiasi Frontend%%'" delete 2>nul

REM 6. Metodo PowerShell per trovare finestre CMD con i nostri comandi
echo Pulizia finale finestre CMD...
powershell -Command "Get-WmiObject Win32_Process | Where-Object { $_.Name -eq 'cmd.exe' -and ($_.CommandLine -like '*cd backend*!BACKEND_PROJECT!.exe*' -or $_.CommandLine -like '*python python_server.py*' -or $_.CommandLine -like '*Backend Server*' -or $_.CommandLine -like '*Pietribiasi Frontend*') } | ForEach-Object { try { Write-Host \"Chiusura finestra CMD PID: $($_.ProcessId)\"; $_.Terminate() } catch {} }" 2>nul

REM 7. Metodo finale: cerca finestre che potrebbero essere tornate al titolo originale
timeout /t 1 >nul
taskkill /fi "WindowTitle eq Backend Server*" /f 2>nul
taskkill /fi "WindowTitle eq Pietribiasi Frontend*" /f 2>nul

REM 8. Termina eventuali processi Python rimasti che potrebbero essere il server
echo Pulizia finale processi Python...
for /f "tokens=2" %%a in ('tasklist /fi "imagename eq python.exe" /fo csv ^| findstr python_server') do (
    taskkill /f /pid %%a 2>nul
)

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
    echo [ATTIVO] Frontend Server Python
) else (
    echo [INATTIVO] Frontend Server Python
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

REM Controlla se il processo Python (python_server.py) è in esecuzione
REM Metodo 1: Cerca python.exe o pythonw.exe
tasklist | findstr "python.exe" >nul 2>&1
if %errorlevel%==0 (
    REM Verifica che sia proprio il nostro script
    wmic process where "name='python.exe' and commandline like '%%python_server.py%%'" get processid >nul 2>&1
    if %errorlevel%==0 set FRONTEND_RUNNING=1
)

REM Metodo 2: Controlla anche pythonw.exe (processo in background)
if "!FRONTEND_RUNNING!"=="0" (
    tasklist | findstr "pythonw.exe" >nul 2>&1
    if %errorlevel%==0 (
        wmic process where "name='pythonw.exe' and commandline like '%%python_server.py%%'" get processid >nul 2>&1
        if %errorlevel%==0 set FRONTEND_RUNNING=1
    )
)

REM Metodo 3: Controlla la porta (alternativo)
if "!FRONTEND_RUNNING!"=="0" (
    netstat -an | findstr ":!FRONTEND_PORT!" | findstr "LISTENING" >nul 2>&1
    if %errorlevel%==0 set FRONTEND_RUNNING=1
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