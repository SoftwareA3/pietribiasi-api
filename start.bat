@echo off
TITLE Pannello di Controllo - PietribiasiApp

:menu
cls
echo.
echo ===========================================
echo  Pannello di Controllo PietribiasiApp
echo ===========================================
echo.
echo  1. Avvia Applicazione (in background)
echo  2. Arresta Applicazione
echo  3. Visualizza Log in tempo reale
echo  4. Ricostruisci Immagini (dopo un aggiornamento)
echo  5. Esci
echo.
set /p choice="Scegli un'opzione: "

if "%choice%"=="1" goto start_app
if "%choice%"=="2" goto stop_app
if "%choice%"=="3" goto logs_app
if "%choice%"=="4" goto build_app
if "%choice%"=="5" exit

goto menu

:start_app
echo Avvio dei container Docker...
docker-compose up -d
echo Applicazione avviata.
echo Frontend disponibile su http://localhost:%FRONTEND_PORT_EXTERN%
pause
goto menu

:stop_app
echo Arresto dei container Docker...
docker-compose down
echo Applicazione arrestata.
pause
goto menu

:logs_app
echo Visualizzazione dei log in tempo reale... (Premi CTRL+C per tornare al menu)
docker-compose logs -f
goto menu

:build_app
echo Arresto e ricostruzione delle immagini Docker...
docker-compose down
docker-compose build --no-cache
echo Ricostruzione completata. Puoi ora avviare l'applicazione.
pause
goto menu