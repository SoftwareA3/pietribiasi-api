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
echo - ps2exe ^(PowerShell to EXE converter^)
echo - PyInstaller ^(Python to EXE converter^)
echo - psutil ^(Python library^)
echo - pywebview ^(Python library^)
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

echo [INFO] Controllo presenza ps2exe...
powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; if ($module) { Write-Host '[OK] ps2exe già installato.' -ForegroundColor Green; exit 0 } else { Write-Host '[INFO] ps2exe non trovato.' -ForegroundColor Yellow; exit 1 } } catch { Write-Host '[INFO] ps2exe non trovato.' -ForegroundColor Yellow; exit 1 }"

if errorlevel 1 (
    echo [INFO] Installazione ps2exe in corso...
    echo [INFO] Configurazione repository PSGallery come trusted...
    
    powershell -Command "try { Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted; Write-Host '[OK] Repository PSGallery configurato.' -ForegroundColor Green } catch { Write-Host '[WARNING] Impossibile configurare PSGallery come trusted.' -ForegroundColor Yellow }"
    
    echo [INFO] Download e installazione ps2exe...
    powershell -Command "try { Install-Module -Name ps2exe -Force -Scope AllUsers -Repository PSGallery -AllowClobber; Write-Host '[OK] ps2exe installato con successo.' -ForegroundColor Green; exit 0 } catch { Write-Host '[ERRORE] Impossibile installare ps2exe: ' $_.Exception.Message -ForegroundColor Red; exit 1 }"
    
    if errorlevel 1 (
        echo [ERRORE] Installazione ps2exe fallita.
        echo [INFO] Tentativo di installazione con modalità CurrentUser...
        
        powershell -Command "try { Install-Module -Name ps2exe -Force -Scope CurrentUser -Repository PSGallery -AllowClobber; Write-Host '[OK] ps2exe installato con successo per l'utente corrente.' -ForegroundColor Green; exit 0 } catch { Write-Host '[ERRORE] Impossibile installare ps2exe anche per CurrentUser: ' $_.Exception.Message -ForegroundColor Red; exit 1 }"
        
        if errorlevel 1 (
            echo [ERRORE] Impossibile installare ps2exe. Verifica:
            echo - Connessione a Internet
            echo - Permessi di amministratore
            echo - Configurazione PowerShell ExecutionPolicy
            pause
            exit /b 1
        )
    )
    
    echo [INFO] Verifica finale installazione ps2exe...
    powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; if ($module) { Write-Host '[OK] ps2exe installato e verificato.' -ForegroundColor Green; Write-Host 'Versione: ' $module.Version -ForegroundColor Cyan } else { Write-Host '[ERRORE] ps2exe non trovato dopo l''installazione.' -ForegroundColor Red; exit 1 } } catch { Write-Host '[ERRORE] Errore durante la verifica di ps2exe.' -ForegroundColor Red; exit 1 }"
    
    if errorlevel 1 (
        echo [ERRORE] Verifica ps2exe fallita.
        pause
        exit /b 1
    )
) else (
    echo [OK] ps2exe già installato.
    powershell -Command "try { $module = Get-Module -ListAvailable -Name ps2exe; Write-Host 'Versione installata: ' $module.Version -ForegroundColor Cyan } catch { Write-Host 'Impossibile ottenere la versione.' -ForegroundColor Yellow }"
)
echo.

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
) else (
    echo [OK] pywebview già installato.
)

echo [INFO] Controllo presenza di PyInstaller...
python -m pip show pyinstaller >nul 2>&1
if errorlevel 1 (
    echo [INFO] PyInstaller non trovato. Installazione in corso...
    echo [INFO] Aggiornamento pip...
    python -m pip install --upgrade pip
    if errorlevel 1 (
        echo [ERRORE] Impossibile aggiornare pip.
        pause
        exit /b 1
    )
    
    echo [INFO] Installazione PyInstaller...
    python -m pip install pyinstaller
    if errorlevel 1 (
        echo [ERRORE] Impossibile installare PyInstaller. Verifica la connessione a Internet o i permessi.
        pause
        exit /b 1
    )
    echo [OK] PyInstaller installato con successo.
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
goto fine

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