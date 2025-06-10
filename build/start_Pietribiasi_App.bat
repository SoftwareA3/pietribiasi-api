@echo off
title Pietribiasi App Launcher
cd /d "%~dp0"

echo Avvio di Pietribiasi App in corso...

REM Avvia il Backend
echo Avvio del Backend Server...
start "Backend" /B .\backend\apiPB.exe --urls="http://192.168.100.113:5245"

REM Avvia il Frontend
echo Avvio del Frontend Server...
start "Frontend" /B powershell -command "npx http-server ./frontend -a 192.168.100.113 -p 8080 --cors -o index.html"

echo.
echo Pietribiasi App e' in esecuzione.
echo Chiudi questa finestra per terminare l'applicazione.
pause >nul

echo Arresto dell'applicazione in corso...
taskkill /F /FI "WINDOWTITLE eq Backend" /T > nul
taskkill /F /FI "WINDOWTITLE eq Frontend" /T > nul
