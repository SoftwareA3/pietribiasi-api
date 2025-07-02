@echo off
setlocal enabledelayedexpansion

set PATH=%~dp0..\Scripts;%PATH%
set BUILD_PATH=..\BuildAndDistr

echo ========================================
echo    PIETRIBIASI APP - SETUP INIZIALE
echo ========================================
echo.
echo Questo script configurerà l'ambiente di sviluppo.
echo.

rem --- Controllo Python ---
echo [INFO] Controllo presenza Python...
python --version >nul 2>&1
if errorlevel 1 (
    echo [ERRORE] Python non trovato!
    echo.
    echo SOLUZIONE: Installa Python manualmente da https://www.python.org/downloads/
    echo - Scarica la versione più recente
    echo - Durante l'installazione, seleziona "Add Python to PATH"
    echo - Dopo l'installazione, riavvia questo script
    echo.
    pause
    exit /b 1
) else (
    for /f "tokens=2" %%i in ('python --version 2^>^&1') do set PYTHON_VERSION=%%i
    echo [OK] Python !PYTHON_VERSION! trovato.
)

echo.

rem --- Aggiornamento pip ---
echo [INFO] Aggiornamento pip...
python -m pip install --user --upgrade pip >nul 2>&1

rem --- Installazione dipendenze ---
echo [INFO] Controllo e installazione dipendenze...
echo.

rem Funzione per controllare e installare un pacchetto
call :check_and_install_package pywebview webview
call :check_and_install_package flask flask
call :check_and_install_package flask-cors flask_cors
call :check_and_install_package pyinstaller PyInstaller

echo.
echo [OK] Tutte le dipendenze sono installate.
echo.
pause
cls

rem --- Menu principale ---
:menu
cls
echo ====================================
echo  PIETRIBIASI APP - BUILD MANAGER
echo ====================================
echo.
echo  Seleziona un'opzione:
echo.
echo  1. Build Completa (build_script.py)
echo  2. Build Solo Frontend (build_script_FE_only.py)
echo  3. Build Entrambi
echo  4. Verifica Dipendenze
echo  5. Esci
echo.
echo ====================================

set /p "scelta=Scelta [1-5]: "
set "scelta=%scelta: =%"

if "%scelta%"=="1" goto build_full
if "%scelta%"=="2" goto build_fe
if "%scelta%"=="3" goto build_all
if "%scelta%"=="4" goto check_deps
if "%scelta%"=="5" goto exit_script

echo [ERRORE] Scelta non valida. Riprova.
pause
goto menu

rem --- Funzioni ---

:check_and_install_package
set package_name=%1
set import_name=%2

echo [INFO] Controllo %package_name%...
python -c "import %import_name%" 2>nul
if errorlevel 1 (
    echo [INFO] Installazione %package_name% in corso...
    python -m pip install --user %package_name%
    if errorlevel 1 (
        echo [ERRORE] Installazione di %package_name% fallita!
        pause
        exit /b 1
    )
    echo [OK] %package_name% installato con successo.
) else (
    echo [OK] %package_name% già installato.
)
goto :eof

:build_full
echo.
echo [INFO] Avvio build completa...
echo ====================================
pushd "%~dp0"
python build_script.py
set build_result=%errorlevel%
popd

if %build_result% neq 0 (
    echo [ERRORE] Build completa fallita!
    pause
    goto menu
)

echo [OK] Build completa completata con successo.
pause
goto menu

:build_fe
echo.
echo [INFO] Avvio build frontend...
echo ====================================
pushd "%~dp0"
python build_script_FE_only.py
set build_result=%errorlevel%
popd

if %build_result% neq 0 (
    echo [ERRORE] Build frontend fallita!
    pause
    goto menu
)

echo [OK] Build frontend completata con successo.
pause
goto menu

:build_all
echo.
echo [INFO] Avvio build completa e frontend...
echo ====================================

pushd "%~dp0"
echo [INFO] Esecuzione build_script.py...
python build_script.py
set build1_result=%errorlevel%

if %build1_result% neq 0 (
    echo [ERRORE] Build completa fallita!
    pause
    goto menu
)

echo [INFO] Esecuzione build_script_FE_only.py...
python build_script_FE_only.py
set build2_result=%errorlevel%
popd

if %build2_result% neq 0 (
    echo [ERRORE] Build frontend fallita!
    pause
    goto menu
)

echo [OK] Entrambe le build completate con successo.
pause
goto menu

:check_deps
cls
echo ====================================
echo  VERIFICA DIPENDENZE
echo ====================================
echo.

echo Controllo Python:
python --version
echo.

echo Controllo dipendenze Python:
python -c "import webview; print('✓ pywebview:', webview.__version__)" 2>nul || echo "✗ pywebview: NON INSTALLATO"
python -c "import flask; print('✓ Flask:', flask.__version__)" 2>nul || echo "✗ Flask: NON INSTALLATO"
python -c "import flask_cors; print('✓ Flask-CORS: OK')" 2>nul || echo "✗ Flask-CORS: NON INSTALLATO"
python -c "import PyInstaller; print('✓ PyInstaller: OK')" 2>nul || echo "✗ PyInstaller: NON INSTALLATO"

echo.
echo ====================================
pause
goto menu

:exit_script
echo.
echo [INFO] Uscita in corso...
goto fine

:fine
echo.
echo [INFO] Operazione terminata.
endlocal
pause
exit /b