# Pietribiasi API

## Controllers
I controllers sono classi che servono ad invocare i metodi HTTP (GET, POST, PUT, DELETE)

### Dipendenze
- Services: Servizio per la scrittura del file di log
- Microsoft.AspNetCore.Authorization: Configurata in 'Programs.cs'
- Dto/Request: Dto per le richieste
- Services/Request/Abstraction: Interfaccia dei servizi di richiesta. Mappatura automatica dei Request Dto in Filters, chiamata dei Repository. Ritornano un Dto

## Authentication
Classe che implementa Basic Authentication, un sistema di autenticazione che chiede allo user di inserire le credenziali "username" e "password", le codifica e le invia al BackEnd per la validazione. La classe si occupa di inviare la richiesta di autenticazione prima di una chiamata all'API e di validare questa richiesta. La validazione viene fatta su un controllo del formato della richiesta e sulla compatibilità tra le stinghe inserite e quelle salvate in appsettings.json.

## Data
In data è salvata la classe che descrive il contesto del database. Tramite questa classe, si può accedere ai modelli e usarli per le interrogazioni/richieste al database, senza dover direttamente recuperare le tabelle ad ogni richiesta.

## Dto
Directory che contiene i file Dto (Data Transfer Object), ossia file di richiesta e risposta all'utente. Questi file sono i contenitori di informazioni che vengono inserite dall'utente (convertite dal JSON del corpo della richiesta) e ritornate dall'applicazione (convertite dal Dto al JSON) all'utente.

### Dto Models
Rappresentano i Dto di risposta o comunque Dto che inviano i record completi in risposta. Contengono tutti i campi contenuti dei rispettivi Models, ma separano la gestione dei dati (Models) dal livello di presentazione all'utente (Dto)

### Dto Request
Rappresentano i Dto di richiesta o comunque Dto che ricevono informazioni parziali. In questo modo, nella richiesta del parametro della proprietà "[FromBody] RequestDto", all'utente saranno nascosti i campi del record e verranno richiesti solo quelli necessari all'operazione
Ad esempio, se il modello rappresenta una tabella User(int Id, string Password) e l'operazione richiede unicamente l'invio dell'id, un Dto UserRequestDto(int Id) chiederà all'utente di inserire unicamente l'id nel corpo della richiesta per la chiamata API.

## Filters
Filters è la directory che contiene i filtri, ossia classi che simulano i file Dto delle richieste. Questo passaggio permette di separare il livello di presentazione (Dto) dal livello dati (Filters) in modo che le richieste siano fatte con un tipo specifico. Può sembrare un'aggiunta superflua, ma nel caso ci sia bisogno di una maggiore gestione o elaborazione dei dati, queste classi possono tornare utili. Ad esempio nel caso in cui, da una richiesta, serva inserire un parametro fisso, si può aggiungere un filtro con i parametri necessari e impostare il parametro fisso nella sezione "logica" dell'applicazione, ossia nei Repository. In questo modo sarà solamente necessario creare il filtro e mappare il Dto nel filtro

## Models
Models è una directory che contiene i modelli generati automaticamente da EntityFramework. Ogni proprietà del modello rappresenta una colonna della tabella. I modelli sono fissi e per tanto vengono usati solo per essere letti dalla logica dell'applicazione quando necessario.

## Repository
Il pattern Repository rappresenta un pattern che recupera le informazioni dal database (Gli si passano dei Filters per le query ad esempio) e si occupa di eseguire la logica (chiamate a altri metodi, query ecc...) per ritornare specifiche informazioni dal database (Models).

### Dipendenze
- Filters: di solito ricevono dei filters come parametri per eseguire query su dati specifici
- Models: di solito ritornano dei models ai servizi perché siano loro a mapparli o trasformarli in Dto

## Services
I Serivices sono classi di servizi che si occupano di svolgere tutte le operazioni intermedie tra la ricezione dei Dto di richiesta al ritorno dei Dto che possono essere inviati all'utente che ha fatto richiesta all'API. Si occupano di:
- Mappare i Dto in entrata nei Filters opportuni;
- Chiamare i metodi dei Repository per gestire la logica della richiesta;
- Convertire i modelli ritornati dai Repository in Dto che si possono restituire all'utente.
Sono necessari per ridurre al minimo il compito dei controllers, che si occuperanno solo di chiamare i servizi

### Dipendenze
- Mappers: per mappare i Dto nei Filters
- Filters: per passarli come parametri ai Services
- Repository: per ricevere i filtri e gestire la logica delle richieste
- Dtos: per ricevere i Dto di richiesta come parametri e ritornare i Dto inviabili all'utente