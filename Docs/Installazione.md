# Installazione e Configurazione

## Download Artefatto
Per installare il prodotto, è necessario scaricare l'artefatto dal Workflow `Build Windown Distribution` su Github.
Se il workflow non è molto recente, è possibile avviarne uno nuovo nella tendina che compare `run workflow` per poi selezionare il branch (di solito develop o main) e premere `Run Workflow`

Una volta completato il Workflow, in fondo alla pagina "Summary" comparirà l'artefatto da scaricare. 

### Presentazione Artefatto
Una volta scaricato, l'artefatto conterrà i seguenti elementi:
- `Backend`: contiene il backend compilato e i file statici del frontend;
- `build.exe`: eseguibile per la costruzione e la configurazione dell'applicazione
- `build.json`: 
- `Documentazione.md`: documentazione sull'applicazione


## Build locale
Alternativamente, se clonata repository e installate le dipendenze necessarie, è possibile creare la distribuzione dell'applicaizone, eseguendo lo script `build.bat`.
La soluzione creata sarà uguale a quella ottenuta scaricando l'artefatto dalla Action Github.

### Dipendenze necessarie
Le dipendenze necessarie per l'esecuzione in locale sono:
- .NET SDK (v.9): 
    - Comando: `winget install Microsoft.DotNet.SDK.9`
    - Link: https://dotnet.microsoft.com/download/dotnet
- Python:
    - Comando: `winget install Python.Python.3.12`
    - Link: https://www.python.org/downloads/
- Pip:
    - Comando: `python -m ensurepip --upgrade`
- PyInstaller:
    - Comando: `pip install pyinstaller`

## Configurazione
Per configurare l'applicazione è necessario modificare il file `build.json`, il quale si occupa di fornire alcune informazioni all'applicazione durante il processo di build. 

Nel file json è possibile configurare:
- La stringa di connessione al database;
- L'IP dove verrà hostato il server;
- La porta sulla quale sarà disponibile il server;
- Se risolvere l'IP in maniera automatica (durante la build inserirà l'IP locale)
- Come chiamare le cartelle di build e di distribuzione (e da dove prendere le informazioni per backend e frontend)
- Nome, versione, descrizione e autore dell'app;
- Se abilitare la build e la distribuzione (per la distribuzione, crea il file compresso solo se l'impostazione è a `true`)

## Installazione
Per installare l'applicazione, una volta effettuate le configurazioni necessarie, bisogna eseguire lo script `build.exe`. 
Il risultato sarà una cartella chiamata come il nome specificato nel file `build.json` e la rispettiva cartella compressa.

All'interno si troveranno i seguenti file:
- Application: contiene il backend compilato e i file statici del frontend;
- `Documentazione.md`: documentazione sull'applicazione;
- `start.bat`: script di avvio per l'applicazione.

Una volta avviato lo script, verrà aperto il terminale dell'applicazione e il link definito all'inizio, collega all'IP e alla porta dove sarà disponibile l'applicazione.
