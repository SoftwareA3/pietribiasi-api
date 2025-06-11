@echo off
setlocal

:: Controlla se Python è già installato
python --version >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Python è già installato.
) else (
    echo Python non è installato. Procedo con l'installazione...

    :: Verifica se winget è disponibile
    winget -v >nul 2>&1
    if %ERRORLEVEL% NEQ 0 (
        echo Errore: winget non è disponibile. Installalo manualmente o aggiorna Windows.
        exit /b 1
    )

    :: Installa Python (ultima versione stabile)
    winget install --id=Python.Python.3 --source=winget --accept-source-agreements --accept-package-agreements
    if %ERRORLEVEL% NEQ 0 (
        echo Errore durante l'installazione di Python.
        exit /b 1
    )
)

:: Esegui lo script Python
echo Eseguo build_script.py...
python build_script.py

endlocal
pause
