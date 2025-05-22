@echo off
echo Avvio dell'applicazione...

REM Avvia il backend
start cmd /k "cd apiPB && dotnet run"

REM Attendi qualche secondo per assicurarti che il backend sia avviato
timeout /t 5

REM Avvia il frontend
start cmd /k "cd Frontend && http-server -a 0.0.0.0 -p 8080 --cors -o index.html -c-1"

REM Mostra gli indirizzi IP disponibili
echo.
echo ----------------------------------------------
echo Per accedere all'applicazione dall'esterno:
echo.
ipconfig | findstr IPv4
echo.
echo Backend: http://192.168.100.113:5245
echo Frontend: http://192.168.100.113:8080
echo ----------------------------------------------