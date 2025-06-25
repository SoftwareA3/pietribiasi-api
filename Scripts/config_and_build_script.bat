@echo off
setlocal enabledelayedexpansion

set PATH=%~dp0..\Scripts;%PATH%

@echo off
echo ========================================
echo    PIETRIBIASI APP - SETUP INIZIALE
echo ========================================
echo.
echo Questo script installerà i prerequisiti necessari per il build.
echo Verranno installati:
REM echo - ps2exe ^(PowerShell to EXE converter^)
echo - Python ^(Python 3.7 o superiore^)
echo - pywebview ^(Python library^)
echo - Flask ^(Python web framework^)
echo - Flask-CORS ^(Cross-Origin Resource Sharing for Flask^)
echo - PyInstaller ^(Python package for creating standalone executables^)
echo.

net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Richiesta privilegi di amministratore...
    echo Questo script ha bisogno dei privilegi di amministratore per installare i moduli PowerShell.
    echo.
    echo Premi un tasto per continuare come amministratore...
    pause >nul
    
    REM Rilancia lo script come amministratore
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

cls

echo =================================
echo  Configurazione e Build Script
echo =================================
echo.

rem --- Blocco di controllo delle dipendenze ---

echo [INFO] Controllo presenza Python...
where python >nul 2>&1
if errorlevel 1 (
    echo [ERRORE] Python non trovato. Per favore, installalo da https://www.python.org/downloads/
    pause
    exit /b 1
)
echo [OK] Python trovato.
echo.

@REM echo [INFO] Controllo presenza ps2exe...
@REM powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; if ($module) { Write-Host '[OK] ps2exe già installato.' -ForegroundColor Green; exit 0 } else { Write-Host '[INFO] ps2exe non trovato.' -ForegroundColor Yellow; exit 1 } } catch { Write-Host '[INFO] ps2exe non trovato.' -ForegroundColor Yellow; exit 1 }"

@REM if errorlevel 1 (
@REM     echo [INFO] Installazione ps2exe in corso...
@REM     echo [INFO] Configurazione repository PSGallery come trusted...
    
@REM     powershell -Command "try { Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted; Write-Host '[OK] Repository PSGallery configurato.' -ForegroundColor Green } catch { Write-Host '[WARNING] Impossibile configurare PSGallery come trusted.' -ForegroundColor Yellow }"
    
@REM     echo [INFO] Download e installazione ps2exe...
@REM     powershell -Command "try { Install-Module -Name ps2exe -Force -Scope AllUsers -Repository PSGallery -AllowClobber; Write-Host '[OK] ps2exe installato con successo.' -ForegroundColor Green; exit 0 } catch { Write-Host '[ERRORE] Impossibile installare ps2exe: ' $_.Exception.Message -ForegroundColor Red; exit 1 }"
    
@REM     if errorlevel 1 (
@REM         echo [ERRORE] Installazione ps2exe fallita.
@REM         echo [INFO] Tentativo di installazione con modalità CurrentUser...
        
@REM         powershell -Command "try { Install-Module -Name ps2exe -Force -Scope CurrentUser -Repository PSGallery -AllowClobber; Write-Host '[OK] ps2exe installato con successo per l'utente corrente.' -ForegroundColor Green; exit 0 } catch { Write-Host '[ERRORE] Impossibile installare ps2exe anche per CurrentUser: ' $_.Exception.Message -ForegroundColor Red; exit 1 }"
        
@REM         if errorlevel 1 (
@REM             echo [ERRORE] Impossibile installare ps2exe. Verifica:
@REM             echo - Connessione a Internet
@REM             echo - Permessi di amministratore
@REM             echo - Configurazione PowerShell ExecutionPolicy
@REM             pause
@REM             exit /b 1
@REM         )
@REM     )
    
@REM     echo [INFO] Verifica finale installazione ps2exe...
@REM     powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; if ($module) { Write-Host '[OK] ps2exe installato e verificato.' -ForegroundColor Green; Write-Host 'Versione: ' $module.Version -ForegroundColor Cyan } else { Write-Host '[ERRORE] ps2exe non trovato dopo l''installazione.' -ForegroundColor Red; exit 1 } } catch { Write-Host '[ERRORE] Errore durante la verifica di ps2exe.' -ForegroundColor Red; exit 1 }"
    
@REM     if errorlevel 1 (
@REM         echo [ERRORE] Verifica ps2exe fallita.
@REM         pause
@REM         exit /b 1
@REM     )
@REM     echo [OK] ps2exe installato e verificato con successo.
@REM     goto menu
@REM ) else (
@REM     echo [OK] ps2exe già installato.
@REM     powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; Write-Host 'Versione installata: ' $module.Version -ForegroundColor Cyan } catch { Write-Host 'Impossibile ottenere la versione.' -ForegroundColor Yellow }"
@REM )
@REM echo.

python -c "import webview" 2>nul
IF %ERRORLEVEL% NEQ 0 (
    echo Installazione pywebview in corso...
    pip install pywebview
    IF %ERRORLEVEL% NEQ 0 (
        echo Errore nell'installazione di pywebview!
        pause
        exit /b 1
    )
    echo [OK] pywebview installato con successo.
    goto menu
) else (
    echo [OK] pywebview già installato.
)

python -c "import flask" 2>nul
IF %ERRORLEVEL% NEQ 0 (
    echo Installazione Flask in corso...
    pip install flask
    IF %ERRORLEVEL% NEQ 0 (
        echo Errore nell'installazione di Flask!
        pause
        exit /b 1
    )
    echo [OK] Flask installato con successo.
    goto menu
) else (
    echo [OK] Flask già installato.
)

python -c "import flask_cors" 2>nul
IF %ERRORLEVEL% NEQ 0 (
    echo Installazione Flask-CORS in corso...
    pip install flask-cors
    IF %ERRORLEVEL% NEQ 0 (
        echo Errore nell'installazione di Flask-CORS!
        pause
        exit /b 1
    )
    echo [OK] Flask-CORS installato con successo.
    goto menu
) else (
    echo [OK] Flask-CORS già installato.
)

python -c "import PyInstaller" 2>nul
IF %ERRORLEVEL% NEQ 0 (
    echo Installazione PyInstaller in corso...
    pip install pyinstaller
    IF %ERRORLEVEL% NEQ 0 (
        echo Errore nell'installazione di PyInstaller!
        pause
        exit /b 1
    )
    echo [OK] PyInstaller installato con successo.
    goto menu
) else (
    echo [OK] PyInstaller già installato.
)

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

set /p "scelta=Scelta [1-4]: "

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
pushd "%~dp0"
python build_script.py
popd
if errorlevel 1 (
    echo [ERRORE] Errore durante l'esecuzione di build_script.py
    pause
    goto menu
)

pushd "%~dp0"
python build_script_FE_only.py
popd
if errorlevel 1 (
    echo [ERRORE] Errore durante l'esecuzione di build_script_FE_only.py
    pause
    goto menu
)

echo [OK] Entrambi gli script sono stati eseguiti con successo.
echo.
pause
goto menu

:build_full
echo.
echo [INFO] Avvio build_script.py...
echo.
pushd "%~dp0"
python build_script.py
popd
if errorlevel 1 (
    echo [ERRORE] Errore durante l'esecuzione di build_script.py
    pause
)
pause
goto menu

:build_fe
echo.
echo [INFO] Avvio build_script_FE_only.py...
echo.
pushd "%~dp0"
python build_script_FE_only.py
popd
if errorlevel 1 (
    echo [ERRORE] Errore durante l'esecuzione di build_script_FE_only.py
    pause
)
pause
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