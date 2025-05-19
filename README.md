# Pietribiasi APP

# Indice 
1. [FrontEnd](#frontend)
   - [Accesso e validazione delle credenziali](#accesso-e-validazione-delle-credenziali)
   - [Home](#home)
   - [Registrazione Ore Commessa](#registrazione-ore-commessa)
   - [Visualizza Ore Registrate](#visualizza-ore-registrate)
   - [Prelievo Materiali Produzione](#prelievo-materiali-produzione)
   - [Visualizza Prelievi Effettuati](#visualizza-prelievi-effettuati) 
   - [Gestione Inventario](#gestione-inventario)
   - [Visualizza Registrazioni Inventario](#visualizza-registrazioni-inventario)
   - [Power User](#power-user)
   - [Sincronizzazione](#sincronizzazione)
   - [Impostazioni](#impostazioni)
2. [Backend](#backend)
   - [Divisione](#divisione)
   - [Controllers](#controllers)
   - [Authentication](#authentication)
   - [Data](#data)
   - [Dto](#dto)
     - [Dto Models](#dto-models)
     - [Dto Request](#dto-request)
   - [Filters](#filters)
   - [Ereditarietà Dto e Filtri](#ereditarietà-dto-e-filtri)
   - [Models](#models)
   - [Repository](#repository)
   - [Services](#services)
   - [Utils](#utils)
   - [Sequenza di esecuzione](#sequenza-di-esecuzione)
     - [Richieste GET](#richieste-get)
     - [Richieste POST](#richieste-post)
     - [Richieste DELETE](#richieste-delete)
     - [Richieste PUT](#richieste-put)
3. [Aggiunta di nuove richieste per il Back End](#aggiunta-di-nuove-richieste-per-il-back-end)
4. [Comandi](#comandi)
   - [Avvio API](#avvio-api)
   - [Scaffolding](#scaffolding)
   - [Scaffolding con stringa di connessione in locale](#scaffolding-con-stringa-di-connessione-in-locale)
   - [Avvio FrontEnd con NodeJs](#avvio-frontend-con-nodejs)

# FrontEnd

## Accesso e validazione delle credenziali
Per l'accesso alla pagina principale dell'applicazione, è necessario l'inserimento di un Codice Addetto. 
Inserita questa password, viene inoltrata una richiesta all'API per il recupero dell'ID del lavoratore. 
Queste due credenziali verranno poi validate attraverso una ricerca di corrispondenza nel database e utilizzate come username(Id) e password(password) per la validazione in Basic Authentication. Queste variabili vengono salvate nella parte FrontEnd tramite dei cookies e rimosse una volta effettuato il Logout.
Questo viene fatto per permettere l'inoltro di altre richieste all'API che richiedono l'autorizzazione tramite Basic Authentication.
I cookies hanno durata massima di 24 ore, di conseguenza dopo un giorno, è necessario rieffettuare l'accesso se la sessione è stata lasciata aperta. 

## Home
La home è la pagina principale del sito alla quale si può accedere inserendo le credenziali correttamente. La pagina si presenta con 9 pulsanti per un utente di tipo **Amministrazione** e con 2 pulsanti in meno per un utente di tipo **Addetto**.
I pulsanti disponibili per tutti gli utenti sono (Titolo - icona):
- **Registrazione Ore Commessa** - 🕛: porta alla pagina della [Registrazione Ore Commessa](#registrazione-ore-commessa)
- **Prerlievo Materiali di Produzione** - 📦: porta alla pagina [Prelievo Materiali Produzione](#prelievo-materiali-produzione)
- **Gestione Inventario** - 📇: porta alla pagina [Gestione Inventario](#gestione-inventario)
- **Visualizza Ore Registrate** - 📄: porta alla pagina [Visualizza Ore Registrate](#visualizza-ore-registrate)
- **Visualizza Prelievi Effettuati** - 🚚: porta alla pagina [Visualizza Prelievi Effettuati](#visualizza-prelievi-effettuati)
- **Visualizza Registrazioni Inventario** - 🔍: porta alla pagina [Visualizza Registrazioni Inventario](#visualizza-registrazioni-inventario)
- **Sincronizza** - 🔄️: attiva la funzione di [Sincronizzazione](#sincronizzazione)

Le operazioni aggiuntive disponibili per gli utenti di tipo amministratore sono:
- **Modalità Power User** - (utente con un "+"): porta alla pagina [Power User](#power-user)
- **Impostazioni** - ⚙️: porta alla pagina [Impostazioni](#impostazioni)

## Viste
Ogni pagina di visualizzazione è dotata di campi compilabili e liste con gli elementi selezionati tramite i filtri. Le liste sono dotate di paginazione. È possibile cambiare pagina premendo il tasto **Successiva** per la pagina successiva e **Precedente** per quella precedente. In alternativa è possibile premere uno dei numeri di pagina per viaggiare rapidamente ad una delle pagine.

## Registrazione Ore Commessa
La pagina per la registrazione delle ore di una commessa si presenta come una serie di campi: 
- Al click sul primo campo, vengono rese visibili in un elenco sotto l’input, tutte le commesse disponibili. Inserendo parte del codice della commessa, vengono filtrate quelle disponibili nell’elenco in modo da restringere il campo.
- Selezionata la commessa, si può inserire l’**Ordine di Produzione** nella stessa maniera e di conseguenza anche la **Lavorazione**.
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
- **Operatore**: il codice dell'operatore che ha effettuato la registrazione
- **Ore**: le ore registrate
- **Data**: la data nella quale sono state salvate le ore

È inoltre disponibile un pallino di colore verde o rosso che indica se la commessa è stata importata (cioè è già stata fatta la sincronizzazione) dal gestionale Mago4 (rosso) o se è stata registrata utilizzando l'applicazione (verde).
In caso la commessa abbia il pallino verde, vengono rese disponibili due operazioni:
- **Modifica**: indicata tramite l'icona ✏️ permette di modificare le ore registrate tramite un input che va poi confermato per l'invio delle modifiche al database
- **Elimina**: indicata tramite l'icona 🗑️ permette di eliminare le ore registrate tramite la pressione del pulsante e la successiva conferma dell'operazione.

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi che sono già stati sincronizzati e poi quelli da sincronizzare. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.

## Prelievo Materiali Produzione
La pagina per il Prelievo di Materiali per Produzione si presenta come una serie di campi: 
- Al click sul primo campo, vengono rese visibili in un elenco sotto l’input, tutte le commesse disponibili. Inserendo parte del codice della commessa, vengono filtrate quelle disponibili nell’elenco in modo da restringere il campo.
- Selezionata la commessa, si può inserire l’**Ordine di Produzione** nella stessa maniera e di conseguenza anche la **Lavorazione**.
- Selezionati tutti i campi, è disponibile un ultimo campo prima dell'inserimento della quantità: questo campo, denominato **Barcode Articolo**, rappresenta il codice dell'articolo e il barcode dell'articolo. Essendo che un articolo può avere più barcode, è possibile inserire porzione del barcode o dell'articolo e visualizzare la lista di autocompletamento dalla quale selezionare l'articolo e il barcode necessari. Inoltre è possibile inserire il barcode completo nell'input e premere "Invio" per compilare in automatico barcode e articolo associato. Quest'operazione garantisce che l'input di una pistola barcode possa incollare il codice di un barcode nell'input e inviarlo in automatico. Selezionando il campo dalla lista di autocompletamento o premento invio con un barcode, il focus si sposta nell'input della selezione delle quantità
- Ogni campo richiede che il precedente sia inserito o selezionato correttamente. Se viene modificato uno dei campi precedenti, quelli successivi, essendone dipendenti, vengono resettati.
- Una tabella in overlay è disponibile alla pressione del pulsante **“Cerca”**, indicato anche tramite l'icona 🔎. Questo pulsante rende disponibile una tabella che elenca tutte le commesse disponibili, se non sono state inserite commesse nel campo **“Codice Commessa”**, altrimenti filtra le commesse in base alle informazioni inserite nel campo e le mostra nella tabella. Selezionando una riga della tabella, vengono compilati in automatico tutti i campi. 
- Quando tutti i campi sono completi, sono da inserire le **Quantità**. Nell'input delle quantità comparirà come valore di default, la quantità prelevabile. Inoltre l'etichetta delle quantità cambierà da "Quantità:" a "Qta. da prelevare: 6000 - Qta. prelevabile: 5983 - UoM: MM", dove la quantità da prelevare è la quantità massima stabilita di prelievo, la quantità prelevabile è la differenza tra la quantità da prelevare e quella già prelevata e UoM determina la sigla dell'unità di misura. 
- Inserite anche le quantità, alla pressione del pulsante **“Aggiungi”**, indicato anche dall'icona ➕,  vengono aggiunte le informazioni recuperate, in una lista temporanea sottostante. 
- Nel caso in cui la quantità inserita superi la quantità da prelevare (ad esempio 6000 nel caso sopra), l'elemento verrà comunque aggiunto, ma verrà mostrato un messaggio per avvisare l'utente.
- Questa lista si resetta all’aggiornamento della pagina, facendo sparire tutte le informazioni che non sono state salvate.
- Ogni informazione salvata nella lista temporanea, è eliminabile tramite l'icona 🗑️. Quest’icona elimina sia l’elemento dalla lista, sia le informazioni che sono state salvate e preparate per il salvataggio.
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella **A3_app_prel_mat**.
In qualsiasi momento è possibile tornare alla home, tramite il pulsante **Annulla**, indicato anche dall'icona ❌. La pressione del pulsante riporta alla homepage, senza salvare le informazioni non salvate, presenti nella lista temporanea.

## Visualizza Prelievi Effettuati
La pagina per la visualizzazione dei prelievi effettuati, si presenta con una serie di campi:
- **Data Da**: rappresenta il filtro per la data dalla quale cercare i prelievi effettuati. L'ora del campo parte dalla mezzanotte del giorno indicato.
- **Data A**: rappresenta il filtro per la data fino alla quale cercare i prelievi effettuati. L'ora del campo arriva fino alle 23 e 59 del giorno indicato.
- **Commessa**: rappresenta il filtro per il codice della commessa con il quale cercare i prelievi effettuati.
- **Barcode Articolo**: rappresenta il filtro per il codice barcode tramite il quale cercare i prelievi effettuati
- **Lavorazione**: rappresenta il filtro per il tipo di lavorazione eseguito, con il quale cercare i prelievi effettuati.
- **Ordine di Produzione**: rappresenta il filtro per il codice dell'ordine di lavoro con il quale cercare i prelievi effettuati.

Ogni campo nel quale è possibile inserire un testo, è dotato di un **autocomplete**: le informazioni vengono caricate preventivamente e vengono filtrate quelle disponibili (eliminando i doppioni) per fornire una lista di selezione che mostra gli elementi disponibili in base alla porzione di input inserita. 

I campi che possiedono un autocomplete sono:
- **Commessa**
- **Barcode Articolo**
- **Ordine di Produzione**
- **Lavorazione**
I campi **Data Da** e **Data A** filtrano in base alla data di salvataggio della quantità prelevata.

Ad ogni campo compilato, è possibile premere il pulsante **Filtra**, indicato tramite l'icona di un imbuto per mostrare la lista delle Quantità Prelevate disponibili.

Quando si cambia campo, viene inviato un segnale che permette allo script di ricevere le informazioni preventivamente e creare la lista di autocompletamento degli altri campi, prima che questi vengano selezionati, eliminando i doppioni in modo da mantenere una lista con elementi tutti diversi.

La lista di elementi filtrati, mostra delle informazioni per ogni elemento. Queste informazioni sono:
- **Comm**: il codice della commessa
- **Lav**: la lavorazione
- **ODP**: l'ordine di produzione
- **Barcode**: il codice barcode
- **Operatore**: il codice dell'operatore che ha effettuato la registrazione
- **Qta**: la quantità prelevata
- **Data**: la data nella quale è stata salvata la quantità prelevata

È inoltre disponibile un pallino verde o rosso che indica se la commessa è stata importata (cioè quei dati sono stati sincronizzati) dal gestionale Mago4 (rosso) o se è stata registrata utilizzando l'applicazione (verde).
In caso la commessa abbia il pallino verde, vengono rese disponibili due operazioni:
- **Modifica**: indicata tramite l'icona ✏️ permette di modificare la quantità prelevata tramite un input che va poi confermato per l'invio delle modifiche al database
- **Elimina**: indicata tramite l'icona 🗑️ permette di eliminare la quantità prelevata tramite la pressione del pulsante e la successiva conferma dell'operazione.

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi che sono già stati sincronizzati e poi quelli da sincronizzare. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.

## Gestione Inventario
La pagina per la Gestione dell'Inventario si presenta con due campi: 
- Inserendo caratteri nel primo campo **Barcode/Articolo**, vengono rese disponibili le prime 5 soluzioni che includono i caratteri digitati. L'input che verrà generato conterrà le informazioni riguardo: l'articolo, la sua descrizione e un barcode associato (se disponibile).
- Inserendo interamente un articolo o un barcode validi e premendo "Invio", l'input verrà compilato in automatico e il focus si sposterà direttamente sulla selezione delle quantità.
- Selezionato il campo, si può inserire la **Quantità Rilevata**. Se l'input **Barcode/Articolo** ha già una quantità precedentemente inventariata, viene inserita questa quantità nella **Quantità Rilevata** in modo da capire quale fosse quella precedente.
- Quando tutti i campi sono completi, alla pressione del pulsante **“Aggiungi”** , indicato anche dall'icona ➕ (o alla pressione di "Invio" nel campo della **Quantità rilevata**), vengono aggiunte le informazioni recuperate, in una lista temporanea sottostante. 
- Questa lista si resetta all’aggiornamento della pagina, facendo sparire tutte le informazioni che non sono state salvate.
- Ogni informazione salvata nella lista temporanea, è eliminabile tramite l'icona 🗑️. Quest’icona elimina sia l’elemento dalla lista, sia le informazioni che sono state salvate e preparate per il salvataggio.
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella **A3_app_inventario**.
In qualsiasi momento è possibile tornare alla home, tramite il pulsante **Annulla**, indicato anche dall'icona ❌. La pressione del pulsante riporta alla homepage, senza salvare le informazioni non salvate, presenti nella lista temporanea.

## Visualizza Registrazioni Inventario
La pagina per la visualizzazione delle registrazioni nell'inventario, si presenta con una serie di campi:
- **Data Da**: rappresenta il filtro per la data dalla quale cercare le registrazioni nell'inventario. L'ora del campo parte dalla mezzanotte del giorno indicato.
- **Data A**: rappresenta il filtro per la data fino alla quale cercare le registrazioni nell'inventario. L'ora del campo arriva fino alle 23 e 59 del giorno indicato.
- **Articolo**: rappresenta il filtro per l'articolo tramite il quale cercare le registrazioni nell'inventario.
- **Barcode Articolo**: rappresenta il filtro per il codice barcode tramite il quale cercare le registrazioni nell'inventario.

Ogni campo nel quale è possibile inserire un testo, è dotato di un **autocomplete**: le informazioni vengono caricate preventivamente e vengono filtrate quelle disponibili (eliminando i doppioni) per fornire una lista di selezione che mostra gli elementi disponibili in base alla porzione di input inserita. 

I campi che possiedono un autocomplete sono:
- **Articolo**
- **Barcode Articolo** se disponibile
I campi **Data Da** e **Data A** filtrano in base alla data di salvataggio della registrazione nell'inventario effettuata.

Ad ogni campo compilato, è possibile premere il pulsante **Filtra**, indicato tramite l'icona di un imbuto per mostrare la lista delle Registrazioni disponibili.

Quando si cambia campo, viene inviato un segnale che permette allo script di ricevere le informazioni preventivamente e creare la lista di autocompletamento degli altri campi, prima che questi vengano selezionati, eliminando i doppioni in modo da mantenere una lista con elementi tutti diversi.

La lista di elementi filtrati, mostra delle informazioni per ogni elemento. Queste informazioni sono:
- **Item**: il codice dell'articolo
- **Desc**: la descrizione dell'articolo
- **BarCode**: il barcode. Questo viene visualizzato solo se disponibile
- **Operatore**: il codice dell'operatore che ha effettuato la registrazione
- **Data**: la data della registrazione effettuata
- **Qta**: la quantità registrata

È inoltre disponibile un pallino verde o rosso che indica se la commessa è stata importata dal gestionale MAGO (rosso) o se è stata registrata utilizzando l'applicazione (verde).
In caso la commessa abbia il pallino verde, vengono rese disponibili due operazioni:
- **Modifica**: indicata tramite l'icona ✏️ permette di modificare la quantità prelevata tramite un input che va poi confermato per l'invio delle modifiche al database.

## Power User
La **Modalità Power User** è una modalità disponibile solamente per gli utenti di tipo "Amministrazione" ed è pertanto visibile come pulsante nella home solo da questi utenti. Per gli utenti di tipo "Addetto" viene invece nascosta. 
Questa modalità consente ad un utente di tipo "Amministrazione" di effettuare l'accesso nei panni di un altro addetto. Questo fa sì che, una volta effettuato correttamente l'accesso, verranno salvati dei cookies con le informazioni dell'utente che si sta simulando. Queste informazioni verranno utilizzate per:
- Modificare l'header: per rendere disponibili le informazioni dell'amministratore e dell'utente che sta simulando
- Effettuare registrazioni che verranno fatte come se le stesse facendo l'addetto che si sta simulando.
- Visualizzare le registrazioni effettuate: le informazioni caricate saranno quelle effettuate dall'utente che si sta simulando.
La pagina della **Modalità Power User** si presenta nella seguente maniera:
- Viene reso disponibile un input nel quale poter digitare parzialmente o interamente il codice dell'utente che si vuole simulare, oppure nome o cognome.
- L'input è dotato di **autocomplete**, quindi all'inserimento di caratteri nell'input, vengono visualizzati i risultati simili che sono selezionabili per completare l'input in automatico.
- È disponibile un pulsante **Cerca**, indicato anche tramite l'icona 🔎, che apre un overlay con una tabella di tutti gli utenti disponibili.
- Una volta selezionato correttamente l'utente, è possibile premere il pulsante **Accedi come Addetto**, che salverà le informazioni e simulerà l'addetto. Verranno anche resi disponibili i pulsanti di navigazione che si trovano nella home, in modod da poter navigare più comodamente alla pagina interessata.
- Una volta effettuato l'accesso come addetto, il pulsante **Accedi come Addetto** viene sostituito con il pulsante **Disconnetti**, che cancella i cookies contenenti le informazioni dell'utente che si sta simulando e ripristina le funzionalità dell'utente corrente. 

## Sincronizzazione
Nella **Home Page** è disponibile un pulsante **Sincronizza**, per ogni tipo di utente, indicato tramite l'icona 🔄️. Questo pulsante recupera le liste di elementi inseriti tramite l'applicazione (quindi quelli registrati dagli utenti e che hanno il pallino verde nelle pagine di visualizzazione) ed effettua la sincronizzazione delle liste, inviandole a Mago4 e segnalandole come "Importate". 
Alla pressione del pulsante, l'icona gira per simulare un caricamento ed il pulsante viene disabilitato.
Al completamento dell'operazione, possono verificarsi due eventi:
- L'operazione è andata a buon fine: l'icona di caricamento scompare e viene sostituita da un'icona ✔️. L'icona si resetta dopo un paio di secondi.  
- L'operazione dà esito negativo: l'icona di caricamento scompare e viene sostituita da un'icona ❌. L'icona si resetta dopo un paio di secondi.

## Impostazioni
Nella **Home Page** è disponibile per gli utenti di tipo **Amministrazione** un pulsante **Impostazioni**, rappresentato da un'icona ⚙️. Alla pressione del pulsante, si viene portati in una pagina, nella quale è possibile modificare i campi delle impostazioni. I campi sono degli input nei quali sono inserite le informazioni per la connessione a Mago4 per le richieste API e alcuni campi aggiuntivi che assegnano valori di default per alcune richieste. 
È possibile modificare ognuno di questi campi. L'inoltro delle modifiche è valido solamente alla pressione del pulsante **Salva**, indicato con un'icona 💾. Se l'invio delle modifiche ha successo, viene mostrato un alert per segnalarlo.

# Backend

## Divisione
Per il BackEnd, la maggior parte delle directory hanno una divisione dei file che dipende dalle tabelle del database che vanno ad interrogare. Per Controllers, Repositories, Services e Mappers esiste un file per ogni tabella/vista interrogata.

## Controllers
I controllers sono classi che servono ad invocare i metodi HTTP (GET, POST, PUT, DELETE). 
I controllers inoltre rappresentano il layer di presentazione, ossia si occupano di esporre l'endpoint, inoltrare la richiesta dell'utente al layer di business (il service in questo caso) e inoltre si occupa di fornire la risposta finale, quindi è l'ultimo gestore delle eccezioni che ritorna messaggi all'utente che ha effettuato la richiesta.

### Dipendenze
- Services: Servizio per la scrittura del file di log
- Microsoft.AspNetCore.Authorization: Configurata in 'Programs.cs'
- Dto/Request: Dto per le richieste
- Services/Request/Abstraction: Interfaccia dei servizi di richiesta. Mappatura automatica dei Request Dto in Filters, chiamata dei Repository. Ritornano un Dto

## Authentication
Classe che implementa Basic Authentication, un sistema di autenticazione che chiede allo user di inserire le credenziali "username" e "password", le codifica e le invia al BackEnd per la validazione. La classe si occupa di inviare la richiesta di autenticazione prima di una chiamata all'API e di validare questa richiesta. La validazione viene fatta su un controllo del formato della richiesta e sulla compatibilità tra le stinghe inserite e quelle salvate in appsettings.json.

## ApiClient
La classe contenuta in ApiClient è un delegato del Controller, che si occupa di stabilire la connessione con l'API di Mago4 e di costruire l'header per effettuare le richieste con il Token fornito tramite il login all'API. In questo senso, il controller si occupa esclusivamente di fare le richieste invocando l'ApiClient (che autonomamente stabilisce la connessione) e inserendo l'endpoint con il corpo della richiesta.

## Data
In data è salvata la classe che descrive il contesto del database. Tramite questa classe, si può accedere ai modelli e usarli per le interrogazioni/richieste al database, senza dover direttamente recuperare le tabelle ad ogni richiesta. La classe che descrive il database è generata automaticamente tramite scaffolding dal database (vedi la sezione [Scaffolding](#scaffolding)).

## Dto
Directory che contiene i file Dto (Data Transfer Object), ossia file di richiesta e risposta all'utente. Questi file sono i contenitori di informazioni che vengono inserite dall'utente (convertite dal JSON del corpo della richiesta) e ritornate dall'applicazione (convertite dal Dto al JSON) all'utente.

### Dto Models
Rappresentano i Dto di risposta o comunque Dto che inviano i record completi in risposta. Contengono tutti i campi contenuti dei rispettivi Models, ma separano la gestione dei dati (Models) dal livello di presentazione all'utente (Dto)

### Dto Request
Rappresentano i Dto di richiesta o comunque Dto che ricevono informazioni parziali. In questo modo, nella richiesta del parametro della proprietà "[FromBody] RequestDto", all'utente saranno nascosti i campi del record e verranno richiesti solo quelli necessari all'operazione
Ad esempio, se il modello rappresenta una tabella User(int Id, string Password) e l'operazione richiede unicamente l'invio dell'id, un Dto UserRequestDto(int Id) chiederà all'utente di inserire unicamente l'id nel corpo della richiesta per la chiamata API.

## Filters
Filters è la directory che contiene i filtri, ossia classi che simulano i file Dto delle richieste. Questo passaggio permette di separare il livello di presentazione (Dto) dal livello dati (Filters) in modo che le richieste siano fatte con un tipo specifico. Può sembrare un'aggiunta superflua, ma nel caso ci sia bisogno di una maggiore gestione o elaborazione dei dati, queste classi possono tornare utili. Ad esempio nel caso in cui, da una richiesta, serva inserire un parametro fisso, si può aggiungere un filtro con i parametri necessari e impostare il parametro fisso nella sezione "logica" dell'applicazione, ossia nei Repository. In questo modo sarà solamente necessario creare il filtro e mappare il Dto nel filtro

## Ereditarietà Dto e Filtri
È stata implementata l'ereditarietà tra Dto di richiesta e Filtri in modo da ridurre il numero di Dto e filtri. L'idea è che attributi comuni a più Dto o Filtri, possono essere ereditate da Dto/Filtri generici e a quelli specifici spetta solo implementare gli attributi mancanti. Questo fa sì che modifiche alle singole classi richiedono più attenzioni in quanto alcune di queste strutture vengono adesso condivise da più classi. In caso è necessario creare Dto/Filtri ulteriori che implementano attributi specifici o ereditano da altri Dto/Filtri e poi si specializzano.

## Models
Models è una directory che contiene i modelli generati automaticamente da EntityFramework, tramite scaffolding dal database (vedi sezione [Scaffolding](#scaffolding)). Ogni proprietà del modello rappresenta una colonna della tabella. I modelli sono fissi e per tanto vengono usati solo per essere letti dalla logica dell'applicazione quando necessario.

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

## Utils
Gli Utils sono classi che servono per supportare alcuni processi. In particolare per gestire gli errori dei controller e scrivere il file di log. I file contenuti in Utils sono due:
- LogService: classe che serve a creare la cartella il file di log e a popolarlo con le informazioni necessarie.
- ResponseHandler: è il gestore delle risposte dei Controller dopo aver ricevuto una richiesta API. Questa classe si occupa di catturare la condizione, scrivere sul file di log e ritornare una risposta. 

In particolare gestisce 3 tipi di situazione:
- BadRequest: il corpo della richiesta all'API è vuoto
- NotFound: il corpo della richiesta conteneva informazioni errate o che non hanno restituito risultati. <strong><i>Su questo punto c'è da fare la premessa del NoContent, ossia quando ci si aspetta che non venga ritornato nulla</i></strong>
- Altre richieste tipo 200: (ad esempio 200 Ok o 201 Created) la richiesta è andata a buon fine. Ritorna il Dto di risposta e lo stampa sul file.

### Dipendenze
- LogService: per scrivere sul file di log

## Sequenza di esecuzione

### Richieste GET
Nel controller, alla richiesta di un GET all'API, non vengono passati argomenti come parametri del metodo oppure viene passato un parametro che prende delle informazioni dalla Route.
**Senza parametri:** Viene creata una variabile che sarà il Dto da ritornare all'utente. Questa variabile viene inizializzata invocando un servizio, che non richiede parametri.
**Con un parametro preso da Route:** Viene creata una variabile che sarà il Dto da ritornare all'utente. Questa variabile viene inizializzata invocando un servizio. Il servizio richiederà un RequestDto come parametro che potrà essere inizializzato prima dell'invocazione del metodo o direttamente creato al passaggio del parametro.

### Richieste POST
Nel controller, alla richiesta di un POST all'API, viene richiesto all'utente dall'interfaccia, di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di gestire la richiesta.

### Richieste DELETE
Nel controller, alla richiesta di una DELETE all'API, viene richiesto all'utente dall'interfaccia di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di utilizzare la richiesta per eliminare una o più righe nella tabella.

### Richieste PUT
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
Per aggiungere una nuova richiesta al Back End, la procedura più efficace è la seguente:
- Aggiornamento delle tabelle del database interessate dalla richiesta tramite lo scaffolding, in modo da aggiornare il **Contesto** del database nell'applicazione e i **Modelli**
- Sistemazione di eventuali dipendenze dalla modifica del contesto del database (ad esempio se la tabella viene aggiornata con nuovi campi o campi rimossi, che richiedono probabilmente modifiche ad altre richieste, mappers, repository ecc...)
- Creazione del **Dto** del modello in modo da separare il modello dalle informazioni richieste e/o ritornate. In alternativa, un **Dto** di richiesta che contenga informazioni per la risposta (non ho distinto request e response) 
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
- La stringa del database è prefissata e alcuni campi probabilmente non servono. Sono da modificare i campi "**Data Source**" con l'IP del database o il nome del Server; "Initial Catalog" con il nome del database; "**User ID**" e "Password" con le credenziali di accesso al server.
- "**--output-dir**" indica la directory di destinazione per il salvataggio dei modelli presi dal database
- "**--context**" indica la directory dove verrà salvato il file che descrive il contesto del database
- "**--context**" con il nome che si vuole dare al file che genererà in automatico la classe derivata da DbContext, classe che descriverà i modelli e si occuperà di interrogare il database. Alla creazione del file, scriverà anche la stringa di connessione al database sul codice, che andrà poi tolta, ma mantenuto un riferimento
- "**--table**" indica il nome delle tabelle che si vogliono recuperare e "simulare" dal database. Si possono inserire tabelle e viste indifferentemente e basta indicare il nome (ad esempio dbo.User diventa User)
- "**--force**" è una direttiva che permette di sovrascrivere dati o file già creati, in modo da non doverli rimuovere a mano.

### Scaffolding con stringa di connessione in locale
Essendo che col metodo precedente è necessario inserire la stringa di connessione e che questa viene poi salvata nel file del contesto del database, .NET mette a disposizione delle soluzioni per "censurare" la stringa di connessione.
``` shell 
dotnet user-secrets init
``` 
è un comando che configura l'applicazione in modo da dirle di cercare o scrivere specifiche informazioni in altri file. Un secret può essere creato attraverso il seguente comando:
``` shell 
dotnet user-secrets set ConnectionStrings:YourDatabaseAlias "Data Source=SERVERNAME\SQLEXPRESS;Initial Catalog=NOMEDATABASE;User ID=user_id;Password=password;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
``` 
Questo comando specifica che esiste un oggetto JSON ConnectionStrings con attributo YourDatabaseAlias che contiene la stringa di connessione specificata. Ciò fa sì che, se l'applicazione trova l'alias specificato in qualche comando (ad esempio nello scaffolding) sostituisce la stringa di connessione con l'alias, mantenendo un riferimento per trovarlo.
In questo caso, la stringa di connessione può essere scritta "a mano" all'interno del file `appsettings.json`.
Il file quindi avrà questa composizione:
```json
{
   "ConnectionStrings": {
      "YourDatabaseAlias": "Data Source=SERVERNAME\\SQLEXPRESS;Initial Catalog=NOMEDATABASE;User ID=user_id;Password=password;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
   },
   // Altre impostazioni
}
```
Il comando per lo scaffolding andrà poi modificato e diventerà come segue:
``` shell
dotnet ef dbcontext scaffold Name=ConnectionStrings:YourDatabaseAlias Microsoft.EntityFrameworkCore.SqlServer --output-dir models_directory --context-dir db_context_directory --context nome_file_dbcontext --table table_1 --table table_2 --force
```
E nel file di contesto del database:
``` C#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:YourDatabaseAlias");
```

### Avvio FrontEnd con NodeJs
Per avviare il FrontEnd, bisogna posizionarsi con terminale nella cartella dov'è situato il FrontEnd e avviarlo tramite il comando ```npx serve .``` che avvia un server statico per testare il FrontEnd.
Comando per installare ```npx```: ```npm install -g serve``` 
