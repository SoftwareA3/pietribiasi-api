# Pietribiasi APP
[![dotnet workflow](https://github.com/SoftwareA3/pietribiasi-api/actions/workflows/dotnet.yml/badge.svg?branch=develop)](https://github.com/SoftwareA3/pietribiasi-api/actions/workflows/dotnet.yml)
# Indice 
1. [Avvio](#avvio)
2. [Funzionalità](#funzionalità)
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

# Avvio
Per l'avvio dell'applicazione, basta spostarsi nella cartella creata dal file di build e eseguire il file `start.bat`. Il file non è altro che un piccolo script che esegue il file `apiPB.exe` nella cartella dove sono situati i file dell'applicazione.
Eseguito il file `start.bat`, verrà aperta una console e avviati i server per il frontend e il backend. Il server backend è realizzato tramite `ASP.NET`, di conseguenza IP e porte del server backend coincidono con quelli del frontend. Al completamento dell'avvio del server, verrà generato l'URL dove sarà disponibile l'applicazione.

All'utente finale che deve utilizzare l'applicazione, basterà collegarsi all'URL dell'applicazione `http://IP:porta` e utilizzare l'applicazione

---

# Funzionalità

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
- **Sincronizza** - 🔄️: porta alla pagina [Sincronizzazione](#sincronizzazione) (se la sincronizzazione è permessa per gli addetti)

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
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella del database.
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

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi da sincronizzare e poi che sono già stati sincronizzati. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.
All'attivazione del toggle, comparirà anche un nuovo input per consentire di filtrare per la data di importazione. Aggiornando la lista, gli elemenenti compariranno in ordine crescente dalla data specificata nell'input.

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
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella del database.
In qualsiasi momento è possibile tornare alla home, tramite il pulsante **Annulla**, indicato anche dall'icona ❌. La pressione del pulsante riporta alla homepage, senza salvare le informazioni non salvate, presenti nella lista temporanea.

Sono state aggiunte due funzionalità aggiuntive per il prelievo dei materiali:
Se la quantità già prelevata sull'ERP è 0, viene abilitato un pulsante **Elimina Materiale** indicato dall'icona 🗑️ che consente di inserire l'articolo che, una volta sincronizzato, verrà eliminato.
Inoltre durante la selezione dell'articolo è sempre disponibile una funzionalità per **Aggiungere un nuovo materiale**, indicato con un'icona ➕.
Se cliccato, il pulsante apre una tabella in sovrimpressione dalla quale è possibile selezionare o cercare un materiale da aggiungere. Selezionato il materiale, viene richiesta una quantità necessaria di materiale da prelevare. Inserita anche quest'ultima, il materiale viene inserito nel form e viene richiesto di inserire la quantità da prelevare. Da qui il procedimento di inserimento e salvataggio è analogo a quello per i prelievi standard.

I materiali aggiunti sono sempre indicati nell'applicazione con un colore verde nella lista temporanea, mentre quelli da eliminare, con un colore rosso.

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

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi da sincronizzare e poi che sono già stati sincronizzati. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.
All'attivazione del toggle, comparirà anche un nuovo input per consentire di filtrare per la data di importazione. Aggiornando la lista, gli elemenenti compariranno in ordine crescente dalla data specificata nell'input.

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
- Per salvare le informazioni presenti nella lista temporanea, è possibile premere il pulsante **“Salva”**, indicato anche dall'icona 💾. Questo passa la lista temporanea ad una chiamata all’API che invia e salva le informazioni nella tabella del database.
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

Per la pagina di visualizzazione è disponibile un **toggle** per visualizzare gli elementi dei quali è già stata fatta la sincronizzazione con Mago4. Attivando il toggle, compariranno prima gli elementi da sincronizzare e poi che sono già stati sincronizzati. Per gli elementi sincronizzati, compariranno anche: il codice, la data e l'ora dell'utente che ha effettuato la sincronizzazione.
All'attivazione del toggle, comparirà anche un nuovo input per consentire di filtrare per la data di importazione. Aggiornando la lista, gli elemenenti compariranno in ordine crescente dalla data specificata nell'input.


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

## Torna all'inizio

Per tornare rapidamente all'inizio di questo documento, puoi cliccare su questo link:

[⬆️ Torna all'inizio](#pietribiasi-app)