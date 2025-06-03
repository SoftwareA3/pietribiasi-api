# Pietribiasi APP
[![dotnet workflow](https://github.com/SoftwareA3/pietribiasi-api/workflows/actions/dotnet.yml/badge.svg?event=push)](https://github.com/SoftwareA3/pietribiasi-api/actions/workflows/dotnet.yml/badge.svg?branch=develop)
# Indice 
1. [FrontEnd](#frontend)
   1. [Accesso e validazione delle credenziali](#accesso-e-validazione-delle-credenziali)
   2. [Home](#home)
   3. [Registrazione Ore Commessa](#registrazione-ore-commessa)
   4. [Visualizza Ore Registrate](#visualizza-ore-registrate)
   5. [Prelievo Materiali Produzione](#prelievo-materiali-produzione)
   6. [Visualizza Prelievi Effettuati](#visualizza-prelievi-effettuati) 
   7. [Gestione Inventario](#gestione-inventario)
   8. [Visualizza Registrazioni Inventario](#visualizza-registrazioni-inventario)
   9. [Power User](#power-user)
   10. [Sincronizzazione](#sincronizzazione)
   11. [Impostazioni](#impostazioni)
2. [Backend](#backend)
   1. [Divisione](#divisione)
   2. [Controllers](#controllers)
   3. [Authentication](#authentication)
   4. [Data](#data)
   5. [Dto](#dto)
       - [Dto Models](#dto-models)
       - [Dto Request](#dto-request)
   6. [Filters](#filters)
   7. [Ereditarietà Dto e Filtri](#ereditarietà-dto-e-filtri)
   8. [Models](#models)
   9. [Repository](#repository)
   10. [Services](#services)
   11. [Utils](#utils)
   12. [Sequenza di esecuzione](#sequenza-di-esecuzione)
       - [Richieste GET](#richieste-get)
       - [Richieste POST](#richieste-post)
       - [Richieste DELETE](#richieste-delete)
       - [Richieste PUT](#richieste-put)
3. [Aggiunta di nuove richieste per il Back End](#aggiunta-di-nuove-richieste-per-il-back-end)
4. [Comandi](#comandi)
   1. [Avvio API](#avvio-api)
   2. [Scaffolding](#scaffolding)
   3. [Scaffolding con stringa di connessione in locale](#scaffolding-con-stringa-di-connessione-in-locale)
   4. [Avvio FrontEnd con NodeJs](#avvio-frontend-con-nodejs)
   5. [Avvio Applicazione tramite il file bash](#avvio-applicazione-tramite-il-file-bash) 

# FrontEnd

## Accesso e validazione delle credenziali
Per l'accesso alla pagina principale dell'applicazione, è necessario l'inserimento di un Codice Addetto. 
Inserita questa password, viene inoltrata una richiesta all'API per il recupero dell'ID del lavoratore. 
Queste due credenziali verranno poi validate attraverso una ricerca di corrispondenza nel database e utilizzate come username(Id) e password(password) per la validazione in Basic Authentication. Queste variabili vengono salvate nella parte FrontEnd tramite dei cookies e rimosse una volta effettuato il Logout.
Questo viene fatto per permettere l'inoltro di altre richieste all'API che richiedono l'autorizzazione tramite Basic Authentication.
I cookies hanno durata massima di 24 ore, di conseguenza dopo un giorno, è necessario rieffettuare l'accesso se la sessione è stata lasciata aperta. 

---

## Home
La home è la pagina principale del sito alla quale si può accedere inserendo le credenziali correttamente. La pagina si presenta con 9 pulsanti per un utente di tipo **Amministrazione** e con 2 pulsanti in meno per un utente di tipo **Addetto**.
I pulsanti disponibili per tutti gli utenti sono (Titolo - icona):
- **Registrazione Ore Commessa** - 🕛: porta alla pagina della [Registrazione Ore Commessa](#registrazione-ore-commessa)
- **Prerlievo Materiali di Produzione** - 📦: porta alla pagina [Prelievo Materiali Produzione](#prelievo-materiali-produzione)
- **Gestione Inventario** - 📇: porta alla pagina [Gestione Inventario](#gestione-inventario)
- **Visualizza Ore Registrate** - 📄: porta alla pagina [Visualizza Ore Registrate](#visualizza-ore-registrate)
- **Visualizza Prelievi Effettuati** - 🚚: porta alla pagina [Visualizza Prelievi Effettuati](#visualizza-prelievi-effettuati)
- **Visualizza Registrazioni Inventario** - 🔍: porta alla pagina [Visualizza Registrazioni Inventario](#visualizza-registrazioni-inventario)
- **Sincronizza** - 🔄️: porta alla pagina [Sincronizzazione](#sincronizzazione)

Le operazioni aggiuntive disponibili per gli utenti di tipo amministratore sono:
- **Modalità Power User** - (👤 con un "+"): porta alla pagina [Power User](#power-user)
- **Impostazioni** - ⚙️: porta alla pagina [Impostazioni](#impostazioni)

---

## Viste
Ogni pagina di visualizzazione è dotata di campi compilabili e liste con gli elementi selezionati tramite i filtri. Le liste sono dotate di paginazione. È possibile cambiare pagina premendo il tasto **Successiva** per la pagina successiva e **Precedente** per quella precedente. In alternativa è possibile premere uno dei numeri di pagina per viaggiare rapidamente ad una delle pagine.

---

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

---

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
Inoltre per ogni elemento importato, è disponibile un pulsante, indicato da un'icona di una lista puntata, per visualizzare dei **messaggi di log** relativi al **MoId**. I messaggi sono visualizzabili solo se disponibili. In caso non lo siano, viene avvertito l'utente tramite un icona ⚠️ e un alert in caso si provi a cliccare il pulsante.  

---

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

---

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
Inoltre per ogni elemento importato, è disponibile un pulsante, indicato da un'icona di una lista puntata, per visualizzare dei **messaggi di log** relativi al **MoId**. I messaggi sono visualizzabili solo se disponibili. In caso non lo siano, viene avvertito l'utente tramite un icona ⚠️ e un alert in caso si provi a cliccare il pulsante.

---

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

---

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

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi che sono già stati sincronizzati e poi quelli da sincronizzare. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.
Inoltre per ogni elemento importato, è disponibile un pulsante, indicato da un'icona di una lista puntata, per visualizzare dei **messaggi di log** relativi al **MoId**. I messaggi sono visualizzabili solo se disponibili. In caso non lo siano, viene avvertito l'utente tramite un icona ⚠️ e un alert in caso si provi a cliccare il pulsante.

---

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

---

## Sincronizzazione
La **sincronizzazione** è una funzionalità che permette di recuperare i dati salvati tramite l'applicazione e inviarli al gestionale **Mago4** per di fatto sincronizzare i dati del gestionale. La pagina si apre con una selezione di schede. La scheda di default è impostata sulla **Sincronizzazione Generale**, indicata anche dall'icona 🌐, la quale dispone del classico pulsante per ritornare alla home e un pulsante per la sincronizzazione dei dati. Alla pressione del pulsante di sincronizzazione, indicato tramite l'icona 🔄️ verrà avviata la procedura di sincronizzazione e l'icona inizierà a roteare. Da qui gli esiti possibili sono due:
- L'operazione ha successo o non ci sono stati dati da aggiornare: l'icona scompare e viene sostituita da un ✔️ e i dati vengono aggiornati
- Durante l'operazione si è verificato un errore: l'icona scompare e viene sostituita da una ❌.
Le icone si resettano dopo un paio di secondi per ritentare la sincronizzazione
Le altre schede della pagina di sincronizzazione, mettono a disposizione la possibilità di sincronizzare per singola funzione, ossia per **Inventario**, **Ore Registrate** e **Prelievi**. Ognuna di queste pagine si presenta in maniera molto simile a quella delle rispettive pagine di visualizzazione, tolto il toggle per la visualizzazione delle operazioni già sincronizzate e la possibilità di modificare o eliminare gli elementi. L'utente può quindi applicare i filtri come avviene nelle pagine di visualizzazione e premere il tasto "Filtra" per confermare i filtri e aggiornare la lista di elementi. Viene messo a disposizione un pulsante **Sincronizza dati** a fianco al pulsante per tornare alla home. Questo pulsante permette l'aggiornamento della lista di elementi filtrati. Alla pressione, viene quindi avviata la procedura di sincronizzaizone **solo** per gli elementi appartenenti alla lista (quindi anche filtrati in caso di applicazione del filtro). 
Per inviare tutte le informazioni di un'operazione, è necessario svuotare tutti i campi filtro e premere il pulsante di sincronizzazione. La procedura di sincronizzazione è analoga a quella indicata per la sincronizzazione generale, ma con i vincoli specificati sopra. 

---

## Impostazioni
Nella **Home Page** è disponibile per gli utenti di tipo **Amministrazione** un pulsante **Impostazioni**, rappresentato da un'icona ⚙️. Alla pressione del pulsante, si viene portati in una pagina, nella quale è possibile modificare i campi delle impostazioni. I campi sono degli input nei quali sono inserite le informazioni per la connessione a Mago4 per le richieste API e alcuni campi aggiuntivi che assegnano valori di default per alcune richieste.
Tutti i campi sono inizialmente disabilitati per evitare modifiche accidentali.
È possibile modificare ognuno di questi campi, spuntando un toggle **Abilita modifica** che abilita tutti i campi degli input per consentire all'utente di inserire un nuovo input. L'inoltro delle modifiche è valido solamente alla pressione del pulsante **Salva**, indicato con un'icona 💾. Se l'invio delle modifiche ha successo, viene mostrato un alert per segnalarlo.
All'interno della pagina è presente un input in particolare che ha degli effetti per gli altri utenti: l'input di selezione **Sincronizzazione globale** serve per abilitare o disabilitare la possibilità che tutti gli utenti possano visualizzare e utilizzare la pagina di sincronizzazione. Se la selezione è su **False**, gli utenti di tipo diverso da **Amministrazione** non vedranno la pagina di sincronizzazione all'interno della home e non potranno quindi utilizzarla. Attivandola, ogni utente può visualizzare il pulsante di sincronizzazione. Gli utenti di tipo diverso da **Amministrazione** non potranno comunque visualizzare tutte le operazioni, ma solo quelle svolte da loro.

---

# Backend

## Divisione
Per il BackEnd, la maggior parte delle directory hanno una divisione dei file che dipende dalle tabelle del database che vanno ad interrogare. Per Controllers, Repositories, Services e Mappers esiste un file per ogni tabella/vista interrogata. L'unica eccezione a questo pattern è rappresentata dalle richieste a Mago4, che sono state raggruppate in un'unico file per comodità

---

## Controllers
I controllers sono classi che servono ad invocare i metodi HTTP (GET, POST, PUT, DELETE). 
I controllers inoltre rappresentano il layer di presentazione, ossia si occupano di esporre l'endpoint, inoltrare la richiesta dell'utente al layer di business (i service in questo caso) e inoltre si occupa di fornire la risposta finale, quindi è l'ultimo gestore delle eccezioni che ritorna messaggi all'utente che ha effettuato la richiesta.

### Dipendenze
- Services: Servizio per la scrittura del file di log
- Microsoft.AspNetCore.Authorization: Configurata in 'Programs.cs'
- Dto/Request: Dto per le richieste
- Services/Request/Abstraction: Interfaccia dei servizi di richiesta. Mappatura automatica dei Request Dto in Filters, chiamata dei Repository. Ritornano un Dto

### Classi e Metodi
#### MagoController
**ROUTE:** `api/mago_api`
Si occupa della gestione di login, logoff, sincronizzazione generale e specifica per le singole operazioni

---

```csharp
   public async Task<IActionResult> Login([FromBody] MagoLoginRequestDto? magoLoginRequestDto)
```
- **Endpoint:** `login`
- **Richiesta:** POST
- **Descrizione:** Effettua il login all'API di Mago4 e ritorna alcune informazioni, tra le quali il token per effettuare le richieste all'API.
- **Parametri:**
   - `MagoLoginRequestDto` un Dto di richiesta contenente le informazioni per effettuare la richiesta di login.
- **Ritorna:** `Task<IActionResult>`
   - `200 Ok` se l'operazione va a buon fine. Ritorna anche il Dto di risposta contenente il token.
   - `400 Bad Request` se la richiesta ha corpo nullo.
   - `404 Not Found` se la richiesta non ha prodotto risultati.

--- 

```csharp
public async Task<IActionResult> LogOff([FromBody] MagoTokenRequestDto tokenRequestDto)
```
- **Endpoint:** `logoff`
- **Richiesta:** POST
- **Descrizione:** Effettua il logoff dall’API di Mago, inviando il Token come parametro.
- **Parametri:**
    - `MagoTokenRequestDto` un Dto di richiesta contenente il token per la richiesta di logoff
- **Ritorna:** `Task<IActionResult>`
    - `200 Ok` se l'operazione va a buon fine.
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il logoff non è riuscito.

---

```csharp
public async Task<IActionResult> Syncronize([FromBody] WorkerIdSyncRequestDto? request)
```
- **Endpoint:** `synchronize_all`
- **Richiesta:** POST
- **Descrizione:** effettua la sincronizzazione dei dati salvati nelle tabelle `A3_Prel_Mat`; `A3_App_Reg_Ore` e `A3_App_Inventario`
- **Parametri:**
    - `WorkerIdSyncRequestDto` Dto di richiesta contenente l’id dell’utente che effettua la sincronizzazione
- **Ritorna:** `Task<IActionResult>`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche le liste delle informazioni che sono state sincronizzate
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la sincronizzazione non è riuscita

---

```csharp
public async Task<IActionResult> SyncRegOreFiltered([FromBody] SyncRegOreFilteredDto request)
```
- **Endpoint:** `sync_reg_ore_filtered`
- **Richiesta:** POST
- **Descrizione:** effettua la sincronizzazione dei dati salvati nelle tabella tabella `A3_app_reg_ore`. Il parametro contiene una lista facoltativa che permette di selezionare record specifici da sincronizzare
- **Parametri:**
    - `SyncRegOreFilteredDto` Dto di richiesta contenente l’id dell’utente che effettua la sincronizzazione e un'eventuale lista di record da sincronizzazre
- **Ritorna:** `Task<IActionResult>`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche le liste delle informazioni che sono state sincronizzate
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la sincronizzazione non è riuscita

---

```csharp
public async Task<IActionResult> SyncPrelMatFiltered([FromBody] SyncPrelMatFilteredDto request)
```
- **Endpoint:** `sync_prel_mat_filtered`
- **Richiesta:** POST
- **Descrizione:** effettua la sincronizzazione dei dati salvati nelle tabella tabella `A3_app_prel_mat`. Il parametro contiene una lista facoltativa che permette di selezionare record specifici da sincronizzare
- **Parametri:**
    - `SyncRegOreFilteredDto` Dto di richiesta contenente l’id dell’utente che effettua la sincronizzazione e un'eventuale lista di record da sincronizzazre
- **Ritorna:** `Task<IActionResult>`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche le liste delle informazioni che sono state sincronizzate
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la sincronizzazione non è riuscita

---

```csharp
public async Task<IActionResult> SyncInventarioFiltered([FromBody] SyncInventarioFilteredDto request)
```
- **Endpoint:** `sync_inventario_filtered`
- **Richiesta:** POST
- **Descrizione:** effettua la sincronizzazione dei dati salvati nelle tabella tabella `A3_app_inventario`. Il parametro contiene una lista facoltativa che permette di selezionare record specifici da sincronizzare
- **Parametri:**
    - `SyncRegOreFilteredDto` Dto di richiesta contenente l’id dell’utente che effettua la sincronizzazione e un'eventuale lista di record da sincronizzazre
- **Ritorna:** `Task<IActionResult>`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche le liste delle informazioni che sono state sincronizzate
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la sincronizzazione non è riuscita

---

#### SettingsController
**ROUTE:** `api/settings`

```csharp
public IActionResult GetSettings()
```
- **Endpoint:** `get_settings`
- **Richiesta:** GET
- **Descrizione:** Recupera le impostazioni salvate nella tabella `A3_App_Settings`
- **Parametri:**
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna le informazioni della tabella
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult EditSettings([FromBody] SettingsDto? request)
```
- **Endpoint:** `edit_settings`
- **Richiesta:** POST
- **Descrizione:** Invia un Dto con le modifiche che vogliono essere effettuate alle impostazioni
- **Parametri:**
    -  `SettingsDto` Dto di richiesta contenente i valori opzionali con i quali modificare il record della tabella delle impostazioni
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche le impostazioni modificate
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la modifica è fallita

---

```csharp
public IActionResult GetSyncGlobalActive()
```
- **Endpoint:** `get_sync_global_active`
- **Richiesta:** GET
- **Descrizione:** Recupera un valore booleano dalla tabella `A3_app_settings` che specifica se le operazioni di sincronizzazione siano utilizzabili da tutti gli utenti o siano visibili solo agli amministratori.
- **Parametri:**
- **Ritorna:** `IActionResult`
    - `200 Ok` se l'operazione va a buon fine. Ritorna il valore richiesto
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### AuthController
**ROUTE:** `api/auth`
**Endpoint:** `validate`
Questa classe serve solo per validare le richieste dell'API che richiedono la validazione tramite BasicAuthentication.

---

#### GiacenzeController
**ROUTE:** `api/giacenze`
Questa classe si occupa del recuper delle informazioni per le operazioni di inventario dalla vista `vw_giacenze`

---

```csharp
public IActionResult GetGiacenze()
```
- **Endpoint:** `get_all`
- **Richiesta:** GET
- **Descrizione:** Recupera tutte le informazioni disponibili dalla vista `vw_giacenze`
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna le informazioni della vista
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### InventarioController
**ROUTE:** `api/inventario`
Questa classe si occupa di effettuare operazioni CRUD sulla tabella `A3_App_Inventario`

---

```csharp
public IActionResult GetInventario()
```
- **Endpoint:** `get_all`
- **Richiesta:** POST
- **Descrizione:** Recupera tutte le informazioni disponibili dalla tabella `A3_App_Inventario`
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna le informazioni della tabella
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult PostInventarioList([FromBody] IEnumerable<InventarioRequestDto>? inventarioRequestDto)
```
- **Endpoint:** `post_inventario`
- **Richiesta:** POST
- **Descrizione:** Richiesta per inserire o modificare un record presente nella tabella. Se il record contiene informazioni simili, ne modifica `BookInv`, altrimenti crea un nuovo record
- **Parametri:**
    -  `IEnumerable<InventarioRequestDto>` Lista di Dto di richiesta contenenti le informazioni per la creazione o la modifica dei record. Ogni elemento della lista rappresenta un record da creare o modificare
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna anche i record modificati
    - `400 Bad Request` se la richiesta ha corpo nullo o vuoto.
    - `404 Not Found` se la richiesta non ha prodotto risultati e l'aggiornamento del contesto è fallito

---

```csharp
public IActionResult GetViewInventario([FromBody] ViewInventarioRequestDto? request)
```
- **Endpoint:** `get_view_inventario`
- **Richiesta:** POST
- **Descrizione:** Usando i parametri opzionali del Dto di richiesta, ritorna i record della tabella `A3_App_Inventario` filtrati per i parametri di richiesta.
- **Parametri:**
    - `ViewInventarioRequestDto` Dto di richiesta contenente i parametri per il filtro  
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna la lista di record filtrati dalla tabella
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult PutViewInventario([FromBody] ViewInventarioPutRequestDto? request)
```
- **Endpoint:** `view_inventario/edit_book_inv`
- **Richiesta:** PUT
- **Descrizione:** Usando l'Id del record da modificare e la quantità, modifica un record dalla tabella `A3_App_Inventario` con la quantità specificata
- **Parametri:**
    - `ViewInventarioRequestDto` Dto di richiesta contenente i parametri per il filtro  
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna il record modificato
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il record non è stato modificato correttamente

---

#### JobController
**ROUTE:** `api/job`
Questa classe si occupa del recupero delle informazioni dalla vista `vw_api_job`

---

```csharp
public IActionResult GetVwApiJobs()
```
- **Endpoint:** `api/job`
- **Richiesta:** GET
- **Descrizione:** Recupera tutte le informazioni dalla vista `vw_api_job`
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna le informazioni della vista
    - `404 Not Found` se la richiesta non ha prodotto risultati 

#### MoStepController
**ROUTE:** `api/mostep`
Questa classe si occupa del recupero delle informazioni per il popolamento dei campi di ricerca per la registrazione delle ore. Interroga la vista `vw_api_mosteps`

---

```csharp
public IActionResult GetVwApiMostepWithJob([FromBody] JobRequestDto? mostepRequestDto)
```
- **Endpoint:** `job`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il codice di commessa come parametro di ricerca
- **Parametri:**
    - `JobRequestDto` Dto di richiesta contenente il codice di commessa
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult GetMostepWithMono([FromBody] MonoRequestDto? mostepMonoRequestDto)
```
- **Endpoint:** `odp`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il codice dell'ordine di lavoro come parametro di ricerca
- **Parametri:**
    - `MonoRequestDto` Dto di richiesta contenente il codice dell'ordine di lavoro e la data di creazione
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult GetMostepWithOperation([FromBody] OperationRequestDto? mostepOperationRequestDto)
```
- **Endpoint:** `odp`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando la lavorazione come parametro di ricerca
- **Parametri:**
    - `OperationRequestDto` Dto di ricerca contenente le informazioni sulla lavorazione
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### MostepsMocomponentController
**ROUTE:** `api/mostepsmocomponent`
Questa classe si occupa del recupero delle informazioni per il popolamento dei campi di ricerca per il prelievo di materiali per la produzione. Interroga la vista `vw_mosteps_mocomponent`

---

```csharp
public IActionResult GetMostepsMocomponentWithJob([FromBody] JobRequestDto? mostepsMocomponentJobRequestDto)
```
- **Endpoint:** `job`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il codice di commessa come parametro di ricerca
- **Parametri:**
    - `JobRequestDto` Dto di ricerca contenente il codice della commessa
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult GetMostepsMocomponentWithMono([FromBody] MonoRequestDto? mostepsMocomponentMonoRequestDto)
```
- **Endpoint:** `mono`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il codice ordine di produzione e la data di creazione come parametro di ricerca
- **Parametri:**
    - `MonoRequestDto` Dto di ricerca contenente il codice dell'ordine di produzione e la data di creazione
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult GetMostepsMocomponentWithOperation([FromBody] OperationRequestDto? mostepsMocomponentOperationRequestDto)
```
- **Endpoint:** `operation`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il tipo di operazione
- **Parametri:**
    - `OperationRequestDto` Dto di ricerca contenente le informazioni sulla lavorazione
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult GetMostepsMocomponentWithBarCode([FromBody] BarCodeRequestDto? mostepsMocomponentBarCodeRequestDto)
```
- **Endpoint:** `barcode`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il barcode
- **Parametri:**
    - `BarCodeRequestDto` Dto di ricerca contenente il codice barcode
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### OmmessageController
**ROUTE:** `api/ommessage`
Questa classe si occupa del recupero dei file di log da Mago4, utilizzando MoId come parametro di ricerca. Interroga la vista `vw_api_ommessages`

---

```csharp
public IActionResult GetOmmessageGroupedByMoId([FromBody] MoIdRequestDto moIdRequestDto)
```
- **Endpoint:** `get_log_from_moid`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla vista, utilizzando il MoId
- **Parametri:**
    - `MoIdRequestDto` Dto di ricerca contenente l'identificativo MoId
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della vista filtrati
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### PrelMatController
**ROUTE:** `api/prel_mat`
Questa classe si occupa di effettuare le operazioni CRUD sulla tabella `A3_app_prel_mat`

---

```csharp
public IActionResult GetAllPrelMat()
```
- **Endpoint:** `get_all`
- **Richiesta:** GET
- **Descrizione:** Recupera tutte le informazioni dalla tabella
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Ritorna i record della tabella
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult PostPrelMatList([FromBody] IEnumerable<PrelMatRequestDto>? a3AppPrelMatRequestDto)
```
- **Endpoint:** `post_prel_mat`
- **Richiesta:** POST
- **Descrizione:** Invia una lista di record da salvare nella tabella
- **Parametri:**
    - `IEnumerable<PrelMatRequestDto>` Lista di Dto contenente i record con le informazioni da salvare nel database
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine. Salva i record nel database
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e la richiesta non salva i record correttamente

---

```csharp
public IActionResult GetViewPrelMat([FromBody] ViewPrelMatRequestDto? a3AppPrelMatRequestDto)
```
- **Endpoint:** `get_view_prel_mat`
- **Richiesta:** POST
- **Descrizione:** Recupera i dati dalla tabella, filtrati per i parametri opzionali inseriti
- **Parametri:**
    - `ViewPrelMatRequestDto` Dto contenente i parametri utilizzati come filtri per recuperare le informazioni dalla tabella
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono ritornati i dati filtrati dalla tabella
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati 

---

```csharp
public IActionResult PutViewPrelMat([FromBody] ViewPrelMatPutRequestDto? a3AppPrelMatRequestDto)
```
- **Endpoint:** `view_prel_mat/edit_prel_qty`
- **Richiesta:** PUT
- **Descrizione:** Dato l'id del record, ne modifica la quantità inserita
- **Parametri:**
    - `ViewPrelMatPutRequestDto` Dto contenente l'identificativo del record salvato nella tabella e la nuova quantità da salvare
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e il record viene modificato correttamente. Viene inoltre ritornato il record modificato
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il record non viene modificato correttamente

---

```csharp
public IActionResult DeletePrelMatId([FromBody] ViewPrelMatDeleteRequestDto? a3AppPrelMatRequestDto)
```
- **Endpoint:** `view_prel_mat/delete_prel_mat_id`
- **Richiesta:** DELETE
- **Descrizione:** Dato l'id del record, elimina il record dalla tabella
- **Parametri:**
    - `ViewPrelMatDeleteRequestDto` Dto di richiesta contenente l'id del record da eliminare
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e il record viene eliminato correttamente. Ritorna inoltre il record eliminato
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il record non viene eliminato correttamente

---

```csharp
public IActionResult GetPrelMatWithComponent([FromBody] ComponentRequestDto? a3AppPrelMatRequestDto)
```
- **Endpoint:** `get_prel_mat_with_component`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla tabella filtrate per component
- **Parametri:**
    - `ComponentRequestDto` Dto di richiesta contenente il codice del componente
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono ritornati i record filtrati dalla tabella
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

#### RegOreController
**ROUTE:** `api/reg_ore`
Questa classe si occupa di effettuare le operazioni CRUD sulla tabella `A3_app_reg_ore`

---

```csharp
public IActionResult GetAllRegOre()
```
- **Endpoint:** `get_all`
- **Richiesta:** GET
- **Descrizione:** Recupera tutte le informazioni dalla tabella
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono ritornati i record della tabella
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult PostRegOreList([FromBody] IEnumerable<RegOreRequestDto>? a3AppRegOreRequestDto)
```
- **Endpoint:** `post_reg_ore`
- **Richiesta:** POST
- **Descrizione:** Salva la lista di record nella tabella
- **Parametri:**
    - `IEnumerable<RegOreRequestDto>` Dto di richiesta contenente la lista di record da salvare nel database
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono inseriti correttamente i record nel database
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e i dati non sono stati salvati correttamente

---

```csharp
public IActionResult GetA3AppRegOre([FromBody] ViewOreRequestDto? a3AppViewOreRequestDto)
```
- **Endpoint:** `view_ore`
- **Richiesta:** POST
- **Descrizione:** Recupera le informazioni dalla tabella, usando i parametri opzionali come filtri
- **Parametri:**
    - `ViewOreRequestDto` Dto di richiesta contenente i filtri opzionali
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono ritornate le informazioni filtrate dalla tabella
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati

---

```csharp
public IActionResult PutA3AppRegOre([FromBody] ViewOrePutRequestDto? a3AppViewOrePutRequestDto)
```
- **Endpoint:** `view_ore/edit_working_time`
- **Richiesta:** PUT
- **Descrizione:** Usando l'id del record da modificare, modifica le ore lavorate
- **Parametri:**
    - `ViewOrePutRequestDto` Dto di richiesta contenente l'id del record da modificare e il valore da modificare
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e il record viene modificato correttamente
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il record non viene modificato correttamente

---

```csharp
public IActionResult DeleteRegOreId([FromBody] ViewOreDeleteRequestDto? a3AppDeleteRequestDto)
```
- **Endpoint:** `view_ore/delete_reg_ore_id`
- **Richiesta:** DELETE
- **Descrizione:** Usando l'id del record da eliminare, elimina il record dalla tabella
- **Parametri:**
    - `ViewOreDeleteRequestDto` Dto di richiesta contenente l'id del record da eliminare
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e il record viene eliminato correttamente
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il record non viene eliminato correttamente

#### WorkerController
**ROUTE:** `api/worker`
Questa classe si occupa di recuperare le informazioni dalla vista `vw_api_workers`

---

```csharp
public IActionResult GetAllWorkers()
```
- **Endpoint:** `api/worker`
- **Richiesta:** GET
- **Descrizione:** Recupera tutte le informazioni dalla vista
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e vengono ritornati tutti i record dalla tabella
    - `404 Not Found` se la richiesta non ha prodotto risultati 

---

```csharp
public IActionResult LoginWithPassword([FromBody] PasswordWorkersRequestDto? passwordWorkersRequestDto)
```
- **Endpoint:** `login`
- **Richiesta:** POST
- **Descrizione:** Usando la password del lavoratore, effettua il login nell'applicazione
- **Parametri:**
    - `PasswordWorkersRequestDto` Dto di richiesta contenente la password del lavoratore
- **Ritorna:** `IActionResult`  
    - `200 Ok` se l'operazione va a buon fine e il lavoratore viene autenticato correttamente
    - `400 Bad Request` se la richiesta ha corpo nullo.
    - `404 Not Found` se la richiesta non ha prodotto risultati e il lavoratore non viene autenticato correttamente

---

## Authentication
Classe che implementa Basic Authentication, un sistema di autenticazione che chiede allo user di inserire le credenziali "username" e "password", le codifica e le invia al BackEnd per la validazione. La classe si occupa di inviare la richiesta di autenticazione prima di una chiamata all'API e di validare questa richiesta. La validazione viene fatta su un controllo del formato della richiesta e sulla compatibilità tra le stinghe inserite e quelle salvate in appsettings.json.

### Classi e Metodi
#### BasicAuthentication
Per quanto riguarda l'unica classe di Authentication, non verranno descritti i metodi, poiché sono metodi necessari solo al controllo e validazione delle credenziali in Basic Authentication, alla conversione e la gestione della risposta di autenticazione.

---

## ApiClient
La classe contenuta in ApiClient è un delegato del Controller, che si occupa di stabilire la connessione con l'API di Mago4 e di costruire l'header per effettuare le richieste con il Token fornito tramite il login all'API. In questo senso, il controller si occupa esclusivamente di fare le richieste invocando l'ApiClient (che autonomamente stabilisce la connessione) e inserendo l'endpoint con il corpo della richiesta.

### Classi e Metodi
#### MagoApiClient
Per quanto riguarda l'unica classe di ApiClient, non verranno descritti i metodi, poiché sono metodi necessari solo alla realizzazione dell'header con il token e alla connessione per le richieste all'API di Mago4.

---

## Data
In data è salvata la classe che descrive il contesto del database. Tramite questa classe, si può accedere ai modelli e usarli per le interrogazioni/richieste al database, senza dover direttamente recuperare le tabelle ad ogni richiesta. La classe che descrive il database è generata automaticamente tramite scaffolding dal database (vedi la sezione [Scaffolding](#scaffolding)).

### Classi e Metodi
#### ApplicationDbContext
Per quanto riguarda l'unica classe di Data, non verranno descritti i metodi, poiché tutto il file è generato in maniera automatica tramite un comando (vedi la sezione [Scaffolding](#scaffolding)) per la realizzazione del contesto del database.

---

## Dto
Directory che contiene i file Dto (Data Transfer Object), ossia file di richiesta e risposta all'utente. Questi file sono i contenitori di informazioni che vengono inserite dall'utente (convertite dal JSON del corpo della richiesta) e ritornate dall'applicazione (convertite dal Dto al JSON) all'utente.

### Classi
Per quanto riguarda i file contenenti le classi dei Dto, non verranno descritte, poiché contengono solamente attributi per la formazione dei Dto dei modelli o dei Dto di richiesta

### Dto Models
Rappresentano i Dto di risposta o comunque Dto che inviano i record completi in risposta. Contengono tutti i campi contenuti dei rispettivi Models, ma separano la gestione dei dati (Models) dal livello di presentazione all'utente (Dto)

### Dto Request
Rappresentano i Dto di richiesta o comunque Dto che ricevono informazioni parziali. In questo modo, nella richiesta del parametro della proprietà "[FromBody] RequestDto", all'utente saranno nascosti i campi del record e verranno richiesti solo quelli necessari all'operazione
Ad esempio, se il modello rappresenta una tabella User(int Id, string Password) e l'operazione richiede unicamente l'invio dell'id, un Dto UserRequestDto(int Id) chiederà all'utente di inserire unicamente l'id nel corpo della richiesta per la chiamata API.

---

## Filters
Filters è la directory che contiene i filtri, ossia classi che simulano i file Dto delle richieste. Questo passaggio permette di separare il livello di presentazione (Dto) dal livello dati (Filters) in modo che le richieste siano fatte con un tipo specifico. Può sembrare un'aggiunta superflua, ma nel caso ci sia bisogno di una maggiore gestione o elaborazione dei dati, queste classi possono tornare utili. Ad esempio nel caso in cui, da una richiesta, serva inserire un parametro fisso, si può aggiungere un filtro con i parametri necessari e impostare il parametro fisso nella sezione "logica" dell'applicazione, ossia nei Repository. In questo modo sarà solamente necessario creare il filtro e mappare il Dto nel filtro

### Classi
Per quanto riguarda le classi dei filter, non verranno descritte, poiché all'interno di queste ci sono solo attributi ripresi dai Dto di richiesta per effettuare le query nei repository

---

## Ereditarietà Dto e Filtri
È stata implementata l'ereditarietà tra Dto di richiesta e Filtri in modo da ridurre il numero di Dto e filtri. L'idea è che attributi comuni a più Dto o Filtri, possono essere ereditate da Dto/Filtri generici e a quelli specifici spetta solo implementare gli attributi mancanti. Questo fa sì che modifiche alle singole classi richiedono più attenzioni in quanto alcune di queste strutture vengono adesso condivise da più classi. In caso è necessario creare Dto/Filtri ulteriori che implementano attributi specifici o ereditano da altri Dto/Filtri e poi si specializzano.

---

## Models
Models è una directory che contiene i modelli generati automaticamente da EntityFramework, tramite scaffolding dal database (vedi sezione [Scaffolding](#scaffolding)). Ogni proprietà del modello rappresenta una colonna della tabella. I modelli sono fissi e per tanto vengono usati solo per essere letti dalla logica dell'applicazione quando necessario.

### Classi
Per quanto riguarda le classi dei Models, non verranno descritte, poiché all'interno di queste ci sono solo attributi generati automaticamente attraverso lo [Scaffolding](#scaffolding)

---

## Repository
Il pattern Repository rappresenta un pattern che si occupa delle interazioni tra l'API e il database. In questo senso, si occupa di reucperare o inserire inserire infomrazioni nel database attraverso, ad esempio, creazione di oggetti da salvare nel database (crea modelli dalle informazioni ricevute in JSON), recupero di informazioni dal database (esecuzione di query tramite i filters passati) o occupandosi della logica (chiamate ad altri metodi, invocazione di stored procedure ecc...).

### Dipendenze
- Filters: di solito ricevono dei filters come parametri per eseguire query su dati specifici
- Models: di solito ritornano dei models ai servizi perché siano loro a mapparli o trasformarli in Dto

---

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

---

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

---

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
Nel controller, alla richiesta di una PUT all'API, viene richiesto all'utente dall'interfaccia di inserire dei parametri specifici. Questi parametri saranno salvati in formato JSON e presi come parametro dal metodo che si occuperà di utilizzare la richiesta per modificare uno o più campi nella tabella.

### Di seguito:
- Nel service viene invocato un mapper che si occuperà di mappare il Dto passato (se presente) nel rispettivo Filter;
- Verrà invocato un Repository che si occuperà della logica della richiesta, eseguendo ad esempio una query o invocando una stored procedure;
- Il Repository ritornerà un Model che, se necessario, verrà mappato dal Service in un Dto specifico per restituire solo alcune informazioni all'utente;
- Altrimenti il Model ritornato verrà mappato nel rispettivo Dto e restituito sotto forma di `IEnumerable`, ossia una raccolta generica
- Nel Controller viene fatta una conversione a `ToList()` del tipo di ritorno (se è una lista);
- Viene fatto un controllo di esistenza sulla lista o l'elemento ritornato;
- Nel caso di `null` o `List.Empty()`, viene ritornato un messaggio `NotFound()` all'utente e viene inserito l'errore nel file di log `API.log`;
- In caso di successo, verrà ritornato un messaggio `Ok()` o `Created()` che ritorerà le informazioni richieste, lo stato di successo e il rispettivo messaggio. Verrà prima aggiunta la richiesta sul file `API.log` con le informazioni restituite all'utente.

---

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

---

## Comandi

### Avvio API
Per avviare l'API, il comando è: ```dotnet run watch```. Questo comando aprirà una porta di comunicazione con l'API.
Per compilare il progetto o l'API nello specifico, bisogna spostarsi nella cartella dove risiede il file `.csprog` e usare il comando `dotnet build`. In caso di errori, il terminale mostrerà solamente gli errori di compilazione e non quelli a runtime.

### Scaffolding
Per eseguire lo scaffolding del database e creare in automatico sia le tabelle che il DbContext, si deve usare il comando:
``` shell
dotnet ef dbcontext scaffold "Data Source=SERVERNAME\SQLEXPRESS;Initial Catalog=NOMEDATABASE;User ID=user_id;Password=password;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" Microsoft.EntityFrameworkCore.SqlServer --output-dir models_directory --context-dir db_context_directory --context nome_file_dbcontext --table table_1 --table table_2 --force
```
- La stringa del database è prefissata e alcuni campi probabilmente non servono. Sono da modificare i campi "**Data Source**" con l'IP del database o il nome del Server; "Initial Catalog" con il nome del database; "**User ID**" e "Password" con le credenziali di accesso al server.
- "**--output-dir**" indica la directory di destinazione per il salvataggio dei modelli presi dal database
- "**--context**" indica la directory dove verrà salvato il file che descrive il contesto del database
- "**--context**" con il nome che si vuole dare al file che genererà in automatico la classe derivata da DbContext, classe che descriverà i modelli e si occuperà di interrogare il database. Alla creazione del file, scriverà anche la stringa di connessione al database sul codice, che andrà poi tolta, ma mantenuto un riferimento
- "**--table**" indica il nome delle tabelle che si vogliono recuperare e "simulare" dal database. Si possono inserire tabelle e viste indifferentemente e basta indicare il nome (ad esempio dbo.User diventa --table User)
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
Questo comando funziona solo per avviare l'applicazione localmente.

### Avvio Applicazione tramite il file bash
Per avviare l'applicazione tramite il file con estensione `.bat` è sufficiente spostarsi nella directory contenente il file. Da lì, bisogna digitare il comando `./start.bat`. Il comando serve per avviare sia il Backend che il Frontend e inizializzare l'applicazione in `index.html`. Momentaneamente nel Frontend in `main.js` ho scritto una funzione per ritornare una stringa con il mio IP che poi viene chiamata in ogni chiamata API prima dell'inserimento della porta e dell'endpoint. Il file è sufficiente per avviare l'applicazione e per tanto la soluzione usata con **NodeJs** è deprecata. Il Frontend ora utilizza `http-server` globalmente per la demo 

``` bash
@echo off
echo Avvio dell'applicazione...

REM Avvia il backend
start cmd /k "cd apiPB && dotnet run"

REM Attendi qualche secondo per assicurarti che il backend sia avviato
timeout /t 5

REM Avvia il frontend
start cmd /k "cd Frontend && http-server -a 0.0.0.0 -p 8080 --cors -o index.html -c-1"

REM Mostra gli indirizzi IP disponibili
echo.
echo ----------------------------------------------
echo Per accedere all'applicazione dall'esterno:
echo.
ipconfig | findstr IPv4
echo.
echo Backend: http://LOCAL_IP:5245
echo Frontend: http://LOCAL_IP:8080
echo ---------------------------------------------- 
```

In caso di modifiche, cambiare:
- La funzione `getIPString()` in `main.js` e adattarla con l'IP per la comunicazione con il backend
- Le stringhe di connessione nel Backend (apiPB) in `Properties/launchsettings.json`
- L'URL aggiunto nel Backend in `apiPB/Program.cs`
- Il file `start.bat` per configurare gli IP per la comunicazione dell'applicazione
