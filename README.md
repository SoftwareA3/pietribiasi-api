# Pietribiasi API

# FrontEnd

## Accesso e validazione delle credenziali
Per l'accesso alla pagina principale dell'applicazione, è necessario l'inserimento di un Codice Addetto. 
Inserita questa password, viene inoltrata una richiesta all'API per il recupero dell'ID del lavoratore. 
Queste due credenziali verranno poi validate attraverso una ricerca di corrispondenza nel database e utilizzate come username(Id) e password(password) per la validazione in Basic Authentication. Queste variabili vengono salvate nella parte FrontEnd in localStorage e rimosse una volta effettuato il Logout. 
Questo viene fatto per permettere l'inoltro di altre richieste all'API che richiedono l'autorizzazione tramite Basic Authentication. 

## Registrazione Ore Commessa
La pagina per la registrazione delle ore di una commessa si presenta come una serie di campi: 
- Al click sul primo campo, vengono rese visibili in un elenco sotto l’input, tutte le commesse disponibili. Inserendo parte del codice della commessa, vengono filtrate quelle disponibili nell’elenco in modo da restringere il campo.
- Selezionata la commessa, si può inserire l’**Ordine di Lavoro** nella stessa maniera e di conseguenza anche la **Lavorazione**.
- Ogni campo richiede che il precedente sia inserito o selezionato correttamente. Se viene modificato uno dei campi precedenti, quelli successivi, essendone dipendenti, vengono resettati.
- Una tabella in overlay è disponibile alla pressione del pulsante **“Cerca”**, indicato anche tramite l'icona 🔎. Questo pulsante rende disponibile una tabella che elenca tutte le commesse disponibili, se non sono state inserite commesse nel campo **“Codice Commessa”**, altrimenti filtra le commesse in base alle informazioni inserite nel campo e le mostra nella tabella. Selezionando una riga della tabella, vengono compilati in automatico tutti i campi. 
- Quando tutti i campi sono completi, sono da inserire le **Ore**. Inserite anche le ore, alla pressione del pulsante **“Aggiungi”**, indicato anche dall'icona ➕,  vengono aggiunte le informazioni recuperate, in una lista temporanea sottostante. 
- Questa lista si resetta all’aggiornamento della pagina, facendo sparire tutte le informazioni che non sono state salvate.
- Ogni informazione salvata nella lista temporanea, è eliminabile tramite l'icona 🗑️. Quest’icona elimina sia l’elemento dalla lista, sia le informazioni che sono state salvate e preparate per il salvataggio.
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella **A3_app_reg_ore**.
In qualsiasi momento è possibile tornare alla home, tramite il pulsante **Annulla**, indicato anche dall'icona ❌. La pressione del pulsante riporta alla homepage, senza salvare le informazioni non salvate, presenti nella lista temporanea.

## Visualizza Ore Registrate
La pagina per la visualizzazione delle ore registrate, si presenta con una serie di campi:
- **Data Da**: rappresenta il filtro per la data dalla quale cercare le ore registrate. L'ora del campo parte dalla mezzanotte del giorno indicato.
- **Data A**: rappresenta il filtro per la data fino alla quale cercare le ore registrate. L'ora del campo arriva fino alle 23 e 59 del giorno indicato.
- **Commessa**: rappresenta il filtro per il codice della commessa con il quale cercare le ore registrate.
- **Lavorazione**: rappresenta il filtro per il tipo di lavorazione eseguito, con il quale cercare le ore registrate.
- **Ordine di Produzione**: rappresenta il filtro per il codice dell'ordine di lavoro con il quale cercare le ore registrate.

Ogni campo nel quale è possibile inserire un testo, è dotato di un **autocomplete**: le informazioni vengono caricate preventivamente e vengono filtrate quelle disponibili (eliminando i doppioni) per fornire una lista di selezione che mostra gli elementi disponibili in base alla porzione di input inserita. 

I campi che possiedono un autocomplete sono:
- **Commessa**
- **Ordine di Produzione**
- **Lavorazione**
I campi **Data Da** e **Data A** filtrano in base alla data di salvataggio dell'ora registrata.

Ad ogni campo compilato, è possibile premere il pulsante **Filtra**, indicato tramite l'icona di un imbuto per mostrare la lista delle Ore Registrate disponibili.

Quando si cambia campo, viene inviato un segnale che permette allo script di ricevere le informazioni preventivamente e creare la lista di autocompletamento degli altri campi, prima che questi vengano selezionati, eliminando i doppioni in modo da mantenere una lista con elementi tutti diversi.

La lista di elementi filtrati, mostra delle informazioni per ogni elemento. Queste informazioni sono:
- **Comm**: il codice della commessa
- **Lav**: la lavorazione
- **ODP**: l'ordine di produzione
- **Ore**: le ore registrate
- **Data**: la data nella quale sono state salvate le ore
È inoltre disponibile un pallino verde o rosso che indica se la commessa è stata importata dal gestionale MAGO (rosso) o se è stata registrata utilizzando l'applicazione (verde).
In caso la commessa abbia il pallino verde, vengono rese disponibili due operazioni:
- **Modifica**: indicata tramite l'icona ✏️ permette di modificare le ore registrate tramite un input che va poi confermato per l'invio delle modifiche al database
- **Elimina**: indicata tramite l'icona 🗑️ permette di eliminare le ore registrate tramite la pressione del pulsante e la successiva conferma dell'operazione

# Backend

## Divisione
Per il BackEnd, la maggior parte delle directory hanno una divisione dei file che dipende dalle tabelle del database che vanno ad interrogare. Per Controllers, Repositories, Services e Mappers esiste un file per ogni tabella/vista interrogata.

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
Il pattern Repository rappresenta un pattern che si occupa delle interazioni tra l'API e il database. In questo senso, si occupa di reucperare o inserire inserire infomrazioni nel database attraverso, ad esempio, creazione di oggetti da salvare nel database (crea modelli dalle informazioni ricevute in JSON), recupero di informazioni dal database (esecuzione di query tramite i filters passati) o occupandosi della logica (chiamate ad altri metodi, invocazione di stored procedure ecc...).

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

## Sequenza di esecuzione

### Richieste GET
Nel controller, alla richiesta di un GET all'API, non vengono passati argomenti come parametri del metodo oppure viene passato un parametro che prende delle informazioni dalla Route.
**Senza parametri:** Viene creata una variabile che sarà il Dto da ritornare all'utente. Questa variabile viene inizializzata invocando un servizio, che non richiede parametri.
**Con un parametro preso da Route:** Viene creata una variabile che sarà il Dto da ritornare all'utente. Questa variabile viene inizializzata invocando un servizio. Il servizio richiederà un RequestDto come parametro che potrà essere inizializzato prima dell'invocazione del metodo o direttamente creato al passaggio del parametro.

### Richieste POST
Nel controller, alla richiesta di un POST all'API, viene richiesto all'utente dall'interfaccia, di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di gestire la richiesta.

## Richieste DELETE
Nel controller, alla richiesta di una DELETE all'API, viene richiesto all'utente dall'interfaccia di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di utilizzare la richiesta per eliminare una o più righe nella tabella.

## Richieste PUT
Nel controller, alla richiesta di una DELETE all'API, viene richiesto all'utente dall'interfaccia di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di utilizzare la richiesta per modificare uno o più campi una o più righe nella tabella.

### Di seguito:
- Nel service viene invocato un mapper che si occuperà di mappare il Dto passato (se presente) nel rispettivo Filter;
- Verrà invocato un Repository che si occuperà della logica della richiesta, eseguendo ad esempio una query o invocando una stored procedure;
- Il Repository ritornerà un Model che, se necessario, verrà mappato dal Service in un Dto specifico per restituire solo alcune informazioni all'utente;
- Altrimenti il Model ritornato verrà mappato nel rispettivo Dto e restituito sotto forma di IEnumerable, ossia una raccolta generica
- Nel Controller viene fatta una conversione a ToList() del tipo di ritorno (se è una lista);
- Viene fatto un controllo di esistenza sulla lista o l'elemento ritornato;
- Nel caso di "null" o "List.Empty()", viene ritornato un messaggio "NotFound()" all'utente e viene inserito l'errore nel file di log API.log;
- In caso di successo, verrà ritornato un messaggio "Ok()" o "Created()" che ritorerà le informazioni richieste, lo stato di successo e il rispettivo messaggio. Verrà prima aggiunta la richiesta sul file API.log con le informazioni restituite all'utente.

## Aggiunta di nuove richieste per il Back End:
Per aggiungere una nuova richiesta al Back End, io farei così:
- Aggiornamento delle tabelle del database interessate dalla richiesta tramite lo scaffolding, in modo da aggiornare il contesto del database nell'applicazione e i modelli
- Sistemazione di eventuali dipendenze dalla modifica del contesto del database (ad esempio se la tabella viene aggiornata con nuovi campi o campi rimossi, che richiedono probabilmente modifiche ad altre richieste, mappers, repository ecc...)
- Creazione del Dto del modello in modo da separare il modello dalle informazioni richieste e/o ritornate 
- Creazione di un file Dto di richiesta
- Creazione del rispettivo filtro
- Creazione dei mappers (dto - filtro; modello - dto, viceversa, ecc...)
- Creazione dei metodi nei repository
- Creazione dei metodi nei servizi
- Creazione della richiesta nel controller
- Eventuali modifiche in Program.cs (ad esempio se viene creato un nuovo file Mapper, Controller ecc...), cioè files che richiedono configurazioni in Program.cs

## Comandi

### Avvio API
Per avviare l'API, il comando è: ```dotnet run watch```. Questo comando aprirà una porta di comunicazione con l'API e renderà accessibile un'interfaccia su quella porta tramite Swagger (o Postman)

### Scaffolding
Per eseguire lo scaffolding del database e creare in automatico sia le tabelle che il DbContext, si deve usare il comando:
``` shell
dotnet ef dbcontext scaffold "Data Source=SERVERNAME\SQLEXPRESS;Initial Catalog=NOMEDATABASE;User ID=user_id;Password=password;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer --output-dir models_directory --context-dir db_context_directory --context nome_file_dbcontext --table table_1 --table table_2 --force
```
- La stringa del database è prefissata e alcuni campi probabilmente non servono. Sono da modificare i campi "Data Source" con l'IP del database o il nome del Server; "Initial Catalog" con il nome del database; "User ID" e "Password" con le credenziali di accesso al server.
- "--output-dir" indica la directory di destinazione per il salvataggio dei modelli presi dal database
- "--context" indica la directory dove verrà salvato il file che descrive il contesto del database
- "--context" con il nome che si vuole dare al file che genererà in automatico la classe derivata da DbContext, classe che descriverà i modelli e si occuperà di interrogare il database. Alla creazione del file, scriverà anche la stringa di connessione al database sul codice, che andrà poi tolta, ma mantenuto un riferimento
- "--table" indica il nome delle tabelle che si vogliono recuperare e "simulare" dal database. Si possono inserire tabelle e viste indifferentemente e basta indicare il nome (ad esempio dbo.User diventa User)
- "--force" è una direttiva che permette di sovrascrivere dati o file già creati, in modo da non doverli rimuovere a mano.
