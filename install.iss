; Script per Inno Setup - PietribiasiApp

[Setup]
AppName=PietribiasiApp
AppVersion=1.0
DefaultDirName={autopf}\PietribiasiApp
DefaultGroupName=PietribiasiApp
OutputBaseFilename=Setup-PietribiasiApp
Compression=lzma2
SolidCompression=yes
WizardStyle=modern

[Files]
; Includi tutti i file necessari per docker-compose
Source: "docker-compose.yml"; DestDir: "{app}"
Source: "start.bat"; DestDir: "{app}"
; Includi l'intero progetto sorgente che docker-compose userà per la build
Source: "apiPB\*"; DestDir: "{app}\apiPB"; Flags: recursesubdirs createallsubdirs
Source: "Frontend\*"; DestDir: "{app}\Frontend"; Flags: recursesubdirs createallsubdirs

[Icons]
; Crea un collegamento sul desktop e nel menu Start
Name: "{group}\PietribiasiApp"; Filename: "{app}\start.bat"
Name: "{autodesktop}\PietribiasiApp"; Filename: "{app}\start.bat"; Tasks: desktopicon

[Tasks]
; Offre all'utente la scelta di creare un'icona sul desktop
Name: "desktopicon"; Description: "Crea un'icona sul desktop"; GroupDescription: "Componenti aggiuntivi:";

[Code]
// Variabili per memorizzare l'input dell'utente
var
  ConfigPage: TInputQueryWizardPage;
  BackendIPInput: TEdit;
  FrontendPortInput: TEdit;
  BackendPortInput: TEdit;

// Funzione per creare la pagina di configurazione personalizzata
procedure InitializeWizard;
begin
  // Crea la pagina del wizard
  ConfigPage := CreateInputQueryPage(wpWelcome,
    'Configurazione Applicazione', 'Inserisci i dettagli per la configurazione di rete.',
    'Specifica gli indirizzi IP e le porte per il corretto funzionamento.');

  // Aggiungi i campi di input
  ConfigPage.Add('URL/IP del Backend (visibile dal frontend):', False);
  ConfigPage.Add('Porta per il Frontend (es. 80 o 8080):', False);
  ConfigPage.Add('Porta per il Backend (es. 5000):', False);
  
  // Imposta valori di default
  ConfigPage.Values[0] := 'http://localhost:5000';
  ConfigPage.Values[1] := '80';
  ConfigPage.Values[2] := '5000';
end;

// Funzione eseguita DOPO che l'installazione dei file è completata
procedure CurStepChanged(CurStep: TSetupStep);
var
  Lines: TStringList;
begin
  if CurStep = ssPostInstall then
  begin
    // Crea e scrive il file .env con i valori inseriti dall'utente
    Lines := TStringList.Create;
    try
      Lines.Add('FRONTEND_URL=' + ConfigPage.Values[0]); // URL per il JS del frontend
      Lines.Add('FRONTEND_PORT_EXTERN=' + ConfigPage.Values[1]);
      Lines.Add('BACKEND_PORT_EXTERN=' + ConfigPage.Values[2]);
      // Aggiungi qui altre variabili come la stringa di connessione al DB
      // Lines.Add('DB_CONNECTION_STRING=...');
      
      // Salva il file nella directory di installazione
      Lines.SaveToFile(ExpandConstant('{app}\.env'));
    finally
      Lines.Free;
    end;
  end;
end;