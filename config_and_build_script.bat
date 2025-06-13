@echo off
setlocal enabledelayedexpansion

:: =================================================================
:: CONTROLLO PRIVILEGI AMMINISTRATORE
:: =================================================================
echo Verificando i privilegi di amministratore...
net session >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERRORE] Questo script richiede privilegi di amministratore.
    echo    Per favore, clicca con il tasto destro sul file .bat e scegli "Esegui come amministratore".
    echo.
    pause
    exit /b 1
)
echo [OK] Privilegi di amministratore confermati.
echo.

echo ===============================================
echo   CONFIGURAZIONE E BUILD AUTOMATICO
echo ===============================================
echo.

:: =================================================================
:: 1. INSTALLAZIONE PYTHON
:: =================================================================
echo [1/4] Controllo installazione Python...
python --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] Python e gia installato.
    python --version
) else (
    echo [INFO] Python non e installato. Procedo con l'installazione...

    winget install --id=Python.Python.3 --source=winget --accept-source-agreements --accept-package-agreements --silent
    if %ERRORLEVEL% NEQ 0 (
        echo [ERRORE] Errore durante l'installazione di Python con winget.
        echo     Prova a installare Python manualmente da: https://www.python.org/downloads/
        pause
        exit /b 1
    )
    
    echo [OK] Python installato con successo!
    
    :: Aggiunge le cartelle di Python al PATH per la sessione corrente
    echo     Aggiornamento PATH in corso...
    set "PYTHON_FOUND="
    for /d %%d in ("%LOCALAPPDATA%\Programs\Python\Python3*") do (
        echo        Trovata installazione Python in "%%d"
        set "PATH=!PATH!;%%d;%%d\Scripts\"
        set "PYTHON_FOUND=1"
    )
    if defined PYTHON_FOUND (
        echo        Aggiunte le cartelle di Python al PATH di questa sessione.
    ) else {
        echo        [ATTENZIONE] Impossibile trovare la cartella di installazione di Python.
    }
    
    :: Verifica che Python sia ora disponibile
    timeout /t 2 >nul
    python --version >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo [ATTENZIONE] Python installato ma non e stato possibile trovarlo. Potrebbe essere necessario riavviare il terminale.
    )
)

echo.

:: =================================================================
:: 2. INSTALLAZIONE NODE.JS
:: =================================================================
echo [2/4] Controllo installazione Node.js...
node --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] Node.js e gia installato.
    node --version
    npm --version
) else (
    echo [INFO] Node.js non e installato. Procedo con l'installazione...
    
    winget install --id=OpenJS.NodeJS.LTS --source=winget --accept-source-agreements --accept-package-agreements --silent
    if %ERRORLEVEL% NEQ 0 (
        echo [ERRORE] Errore durante l'installazione di Node.js con winget.
        echo     Prova a installare Node.js manualmente da: https://nodejs.org/
        pause
        exit /b 1
    )
    
    echo [OK] Node.js installato con successo!
    
    :: Aggiunge la cartella di Node.js al PATH per la sessione corrente
    echo     Aggiornamento PATH in corso...
    set "NODE_INSTALL_DIR=%ProgramFiles%\nodejs"
    if exist "!NODE_INSTALL_DIR!\node.exe" (
        set "PATH=!PATH!;"!NODE_INSTALL_DIR!"
        echo        Aggiunto "!NODE_INSTALL_DIR!" al PATH di questa sessione.
    ) else (
        echo        [ATTENZIONE] Impossibile trovare Node.js in "!NODE_INSTALL_DIR!".
    )

    :: Verifica che Node.js sia ora disponibile
    timeout /t 2 >nul
    node --version >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo [ATTENZIONE] Node.js installato ma non e stato possibile trovarlo. Potrebbe essere necessario riavviare il terminale.
    )
)

echo.

:: =================================================================
:: 3. INSTALLAZIONE HTTP-SERVER
:: =================================================================
echo [3/4] Controllo installazione http-server...
npm list -g http-server --depth=0 >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] http-server e gia installato globalmente.
) else (
    echo [INFO] http-server non e installato. Procedo con l'installazione...
    
    npm install -g http-server
    if %ERRORLEVEL% NEQ 0 (
        echo [ERRORE] Errore durante l'installazione di http-server.
        echo     Prova a eseguire manualmente: npm install -g http-server
        pause
        exit /b 1
    )
    
    echo [OK] http-server installato con successo!
)

echo.

:: =================================================================
:: 4. VERIFICA FINALE
:: =================================================================
echo [4/4] Verifica finale dipendenze...
set "DEPENDENCIES_OK=1"
:: Aggiunge il path dei pacchetti globali di NPM per rendere http-server visibile
for /f "delims=" %%i in ('npm prefix -g') do set "PATH=!PATH!;%%i"

python --version >nul 2>&1 || (echo [ERRORE] Python non disponibile & set DEPENDENCIES_OK=0)
node --version >nul 2>&1 || (echo [ERRORE] Node.js non disponibile & set DEPENDENCIES_OK=0)
http-server --version >nul 2>&1 || (echo [ERRORE] http-server non disponibile & set DEPENDENCIES_OK=0)

if "%DEPENDENCIES_OK%"=="0" (
    echo.
    echo [ERRORE] Non tutte le dipendenze sono disponibili.
    echo     Riavvia il terminale come Amministratore ed esegui nuovamente questo script.
    echo.
    pause
    exit /b 1
)

echo [OK] Tutte le dipendenze sono installate e disponibili!
echo.
echo ===============================================
echo   RIEPILOGO VERSIONI
echo ===============================================
echo Python:
python --version 2>nul || echo   Non disponibile
echo.
echo Node.js:
node --version 2>nul || echo   Non disponibile
npm --version 2>nul || echo   npm non disponibile
echo.
echo http-server:
http-server --version 2>nul || echo   Non disponibile
echo ===============================================
echo.

:: Esegui lo script Python di build
echo Avvio del processo di build...
echo.
python build_script.py

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERRORE] Il processo di build e fallito.
    echo     Controlla i messaggi di errore sopra per maggiori dettagli.
    pause
    exit /b 1
)

echo.
echo [OK] Processo completato con successo!
echo.

endlocal
pause
