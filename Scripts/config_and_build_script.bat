@echo off
setlocal enabledelayedexpansion

cls

echo =================================
echo  Configurazione e Build Script
echo =================================
echo.

rem --- Blocco di controllo delle dipendenze ---

echo [INFO] Controllo presenza Python...
where python >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERRORE] Python non trovato. Per favore, installalo da https://www.python.org/downloads/
    pause
    exit /b
)
echo [OK] Python trovato.
echo.

echo.
echo ---------------------------------
echo  Configurazione completata.
echo ---------------------------------
pause
cls


rem --- Menu di scelta ---

:menu
cls
echo ==========================
echo  Seleziona un'opzione:
echo ==========================
echo.
echo  1. Esegui build_script.py (Build Completa)
echo  2. Esegui build_script_FE_only.py (Solo Frontend)
echo  3. Esegui entrambi gli script (Build Completa e Frontend)
echo  4. Esci
echo.
echo ==========================

set /p "scelta=Scelta [1-3]: "

rem Rimuove eventuali spazi inseriti dall'utente
set "scelta=%scelta: =%"

if "%scelta%"=="1" goto build_full
if "%scelta%"=="2" goto build_fe
if "%scelta%"=="3" goto build_all
if "%scelta%"=="4" goto exit_script

echo Scelta non valida. Riprova.
pause
goto menu


rem --- Sezioni di esecuzione ---

:build_all
echo.
echo [INFO] Avvio build_script.py e build_script_FE_only.py...
echo.
python build_script.py
python build_script_FE_only.py
echo [OK] Entrambi gli script sono stati eseguiti con successo.
echo.
goto fine

:build_full
echo.
echo [INFO] Avvio build_script.py...
echo.
python build_script.py
goto menu

:build_fe
echo.
echo [INFO] Avvio build_script_FE_only.py...
echo.
python build_script_FE_only.py
goto menu

:exit_script
echo.
echo Uscita in corso...
goto fine


rem --- Fine dello script ---

:fine
echo.
echo Operazione terminata.
endlocal
pause
exit /b
