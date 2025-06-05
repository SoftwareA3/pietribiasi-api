import { fetchWithAuth } from "./fetch.js";
import { setupAutocomplete } from "./autocomplete.js";
import {createPagination} from "./pagination.js";
import { getCookie } from "./cookies.js";
import { getIPString, parseDateTime } from "./main.js";

// Variabili globali per mantenere lo stato
let globalAllData = null;
let showImportedItems = false;
let filteredList = [];
let commessaList = [];
let lavorazioneList = [];
let odpList = [];
const actionType = 2051538945; // Action Type per la registrazione ore

document.addEventListener("DOMContentLoaded", async function() {
    // Recupera gli elementi del DOM:
    const filterDataDa = document.getElementById("filter-ore-data-da");
    const filterDataA = document.getElementById("filter-ore-data-a");
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-ore-odp");
    const filterOreSubmit = document.getElementById("filter-ore-submit");
    const commessaAutocompleteList = document.getElementById("filter-ore-commessa-autocomplete-list");
    const lavorazioneAutocompleteList = document.getElementById("filter-ore-lavorazione-autocomplete-list");
    const odpAutocompleteList = document.getElementById("filter-ore-odp-autocomplete-list");
    const oreList = document.getElementById("ore-list");
    const noContent = document.getElementById("nocontent");
    const showImportedToggle = document.getElementById("show-imported");
    const importedData = document.getElementById("filter-ore-data-imp");
    const importedDataGroup = document.getElementById("filter-ore-imp-group");

    oreList.classList.add("hidden");
    noContent.classList.remove("hidden");

    // Carica i dati iniziali
    try {
        const user = JSON.parse(getCookie("userInfo"));
        const puUser = JSON.parse(getCookie("pu-User"));
        //console.log("user:", user.tipoUtente);
        if(user && user.tipoUtente === "Amministrazione" && !puUser) 
        {
            filteredList = await fetchAllViewOre();
            populateOreList(filteredList);
            //console.log("Dati iniziali caricati:", filteredList);
        }
        else if(user && user.tipoUtente === "Amministrazione" && puUser)
        {
            // Se l'utente è un amministratore e ha effettuato il login come addetto, mostra i dati dell'addetto
            const workerId = {
                workerId: puUser.workerId
            };
            console.log("ID utente:", workerId);
            const response = await fetchViewOre(workerId);
            if (response && response.length > 0) {
                filteredList = response;
                populateOreList(filteredList);
                //console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", workerId);
                alert("Nessun dato trovato per questo utente. Registrare prima delle ore");
            }
        }
        else {
            // Il tipo è un addetto: filtro in base al codice utente
            const userId = {
                workerId: user.workerId
            };
            console.log("ID utente:", userId);
            const response = await fetchViewOre(userId);
            if (response && response.length > 0) {
                filteredList = response;
                populateOreList(filteredList);
                //console.log("Dati iniziali caricati:", filteredList);
            } else {
                console.error("Nessun dato trovato per l'utente:", userId);
                alert("Nessun dato trovato per l'utente.");
            }
        }

        // Inizializza le liste per l'autocomplete
        await refreshAutocompleteData();
    } catch (error) {
        console.error("Errore durante il caricamento iniziale:", error);
        alert("Si è verificato un errore durante il caricamento iniziale dei dati.");
    }

    // Eventi di modifica dei filtri
    filterDataDa.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterDataA.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });

    filterCommessa.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterLavorazione.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    filterOrdineDiProduzione.addEventListener("change", async function() {
        setTimeout(async () => {
            await refreshAutocompleteData();
        }, 200);
    });

    // Setup pulsante di filtro
    filterOreSubmit.addEventListener("click", async function() {
        try {
            console.log("Pulsante filtro premuto");
            
            // Crea l'oggetto filtro
            const filteredObject = createFilterObject();
            console.log("Filtro applicato:", filteredObject);
            
            // Esegue la chiamata API con i filtri
            const results = await fetchViewOre(filteredObject);
            //console.log("Risultati ricevuti:", results);
            
            // Aggiorna la lista filtrata e popola la vista
            filteredList = results;
            populateOreList(filteredList);
            
            // Aggiorna le liste per l'autocomplete con i nuovi dati
            await refreshAutocompleteData();
        } catch (error) {
            console.error("Errore durante il filtraggio:", error);
            alert("Si è verificato un errore durante il recupero dei dati.");
        }
    });

    showImportedToggle.addEventListener("change", async function() {
        showImportedItems = this.checked;
        if(showImportedItems) {
            importedDataGroup.classList.remove("hidden");
        } else {
            importedDataGroup.classList.add("hidden");
        }
        populateOreList(filteredList);

        await refreshAutocompleteData();
    });

    importedData.addEventListener("change", async function() {
        await refreshAutocompleteData();
    });
});

// Funzione per creare l'oggetto filtro dai valori degli input
function createFilterObject() {
    const filterDataDa = document.getElementById("filter-ore-data-da");
    const filterDataA = document.getElementById("filter-ore-data-a");
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-ore-odp");
    const filterImported = document.getElementById("filter-ore-data-imp");
    const showImportedToggle = document.getElementById("show-imported");
    
    const filteredObject = {};

    const user = JSON.parse(getCookie("userInfo"));
    if(user && user.tipoUtente === "Addetto")
    {
        filteredObject.workerId = user.workerId;
    }

    const puUser = JSON.parse(getCookie("pu-User"));
    if(puUser && puUser.workerId)
    {
        filteredObject.workerId = puUser.workerId;
    }
    
    if (filterDataDa.value) {
        const fromDate = new Date(filterDataDa.value);
        fromDate.setHours(0, 0, 0, 0); // Imposta l'ora a mezzanotte
        filteredObject.fromDateTime = fromDate.toISOString().slice(0, -1); // Rimuove la "Z" che crea problemi con le chiamate all'API
    }
    
    if (filterDataA.value) {
        const toDate = new Date(filterDataA.value);
        toDate.setHours(23, 59, 59, 999); // Imposta l'ora alle 23:59:59.999
        filteredObject.toDateTime = toDate.toISOString().slice(0, -1);
    }

    if (showImportedToggle.checked == true) {
        if(filterImported.value) {
            const importedDate = new Date(filterImported.value);
            filteredObject.dataImp = importedDate.toISOString().slice(0, -1); // Rimuove la "Z" che crea problemi con le chiamate all'API
        }
    }

    if(filterCommessa.value) filteredObject.job = filterCommessa.value;
    if(filterLavorazione.value) filteredObject.operation = filterLavorazione.value;
    if(filterOrdineDiProduzione.value) filteredObject.mono = filterOrdineDiProduzione.value;
    if(showImportedToggle) filteredObject.imported = showImportedToggle.checked ? true : false;
    
    return filteredObject;
}

// Funzione per aggiornare le liste per l'autocomplete
async function refreshAutocompleteData() {
    const filterCommessa = document.getElementById("filter-ore-commessa");
    const filterLavorazione = document.getElementById("filter-ore-lavorazione");
    const filterOrdineDiProduzione = document.getElementById("filter-ore-odp");
    const commessaAutocompleteList = document.getElementById("filter-ore-commessa-autocomplete-list");
    const lavorazioneAutocompleteList = document.getElementById("filter-ore-lavorazione-autocomplete-list");
    const odpAutocompleteList = document.getElementById("filter-ore-odp-autocomplete-list");
    
    // Ottiene i dati per l'autocomplete
    const filteredObject = createFilterObject();
    var tempData = await fetchViewOre(filteredObject);

    // Effettua un controllo sul toggle per mostrare solo gli elementi dell'autocomplete che sono effettivamente nella lista
    if(showImportedItems === false) tempData = tempData.filter(item => item.imported === false);
    
    // Estrae i valori unici per ciascuna lista
    commessaList = extractUniqueValues(tempData, 'job');
    lavorazioneList = extractUniqueValues(tempData, 'operation');
    odpList = extractUniqueValues(tempData, 'mono');
    
    console.log("Liste per autocomplete aggiornate:", {
        commesse: commessaList,
        lavorazioni: lavorazioneList,
        odp: odpList
    });
    
    // Configura l'autocomplete per ciascun campo
    setupAutocomplete(filterCommessa, commessaAutocompleteList, commessaList);
    setupAutocomplete(filterLavorazione, lavorazioneAutocompleteList, lavorazioneList);
    setupAutocomplete(filterOrdineDiProduzione, odpAutocompleteList, odpList);
}

// Funzione per estrarre valori unici da un array di oggetti
function extractUniqueValues(data, field) {
    const uniqueValues = [];
    const valueSet = new Set();
    
    if (data && data.length > 0) {
        data.forEach(item => {
            if (item[field] && !valueSet.has(item[field])) {
                valueSet.add(item[field]);
                uniqueValues.push({
                    [field]: item[field],
                    display: item[field].toString()
                });
            }
        });
    }
    
    return uniqueValues;
}

async function fetchAllViewOre() {
    if(globalAllData) return globalAllData;
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/reg_ore/get_all`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        globalAllData = await request.json();
        return globalAllData;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchViewOre(filteredObject) {
    console.log("Chiamata API con filtri:", filteredObject);

    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/reg_ore/view_ore`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(filteredObject),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const info = await request.json();
        return info;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function populateOreList(data) {
    const oreList = document.getElementById("ore-list");
    const noContent = document.getElementById("nocontent");
    var paginationControls = document.querySelector('.pagination-controls');
    
    // Pulisce la lista attuale
    oreList.innerHTML = "";
    noContent.classList.add("hidden");

    let displayData = showImportedItems === true ? data : data.filter(item => item.imported === false || item.imported === "0");

    // Controlla se la lista è vuota
    if (!data || displayData.length === 0) {
        oreList.classList.add("hidden");
        noContent.classList.remove("hidden");
        if(paginationControls != null)
        {
            paginationControls.classList.add("hidden");
        }
        return;
    }
    
    // Mostra la lista e nascondi il messaggio "nessun elemento"
    oreList.classList.remove("hidden");
    noContent.classList.add("hidden");
    
    // Popola la lista con gli elementi
    await Promise.all(displayData.map(async (item) => {
        const li = document.createElement("li");
        li.dataset.id = item.regOreId; // Aggiunge un data attribute per identificare l'elemento
        
        // Formatta la data in un formato leggibile
        const savedDate = new Date(item.savedDate);
        const formattedDate = savedDate.toLocaleDateString("it-IT");
        
        // Crea il contenuto dell'elemento
        const itemContent = document.createElement("div");
        itemContent.className = "item-content";

        const isImported = item.imported === false || item.imported === "0" ? false : true;
        
        const statusIndicator = document.createElement("div");
        statusIndicator.className = `status-indicator ${isImported === true ? 'status-closed' : 'status-open'}`;
        statusIndicator.title = isImported === true ? 'Importato' : 'Modificabile';
        itemContent.appendChild(statusIndicator);

        const convertedTime = (item.workingTime / 3600);

        // Estrai solo la data e l'orario da dataImp nel formato richiesto
        const parsedDateTime = parseDateTime(item.dataImp);
        
        // Aggiunge le informazioni dell'elemento
        itemContent.innerHTML += `
            <div><strong>Comm:</strong> ${item.job} </div>
            <div><strong>Lav:</strong> ${item.operation} </div> 
            <div><strong>ODP:</strong> ${item.mono} </div>
            <div><strong>Operatore:</strong> ${item.workerId} </div>
            <div><strong>Data:</strong> ${formattedDate} </div>
            <div><strong>Ore:</strong> <span class="ore-value" id="ore-value-${item.regOreId}">${convertedTime}</span> </div>
            ${isImported === true ? `<div><strong>Importato il:</strong> ${parsedDateTime.date} alle ${parsedDateTime.time} </div>` : ''}
            ${isImported === true ? `<div><strong>Importato da:</strong> ${item.userImp} </div>` : ''}
        `;
        
        li.appendChild(itemContent);
        
        // Aggiunge i pulsanti di azione solo se imported è 0
        if (!isImported) {
            const itemActions = document.createElement("div");
            itemActions.className = "item-actions";
            
            // Container per il campo di modifica delle ore (inizialmente nascosto)
            const editContainer = document.createElement("div");
            editContainer.className = "edit-ore-container hidden";
            editContainer.id = `edit-container-${item.regOreId}`;
            
            // Campo input per la modifica
            const editInput = document.createElement("input");
            editInput.type = "number";
            editInput.min = "0.1";
            editInput.step = "0.1";
            editInput.className = "edit-ore-input";
            editInput.id = `edit-ore-input-${item.regOreId}`;
            editInput.value = item.workingTime;
            
            // Pulsante di conferma modifica
            const confirmButton = document.createElement("button");
            confirmButton.className = "button-icon confirm option-button";
            confirmButton.title = "Conferma modifica";
            confirmButton.innerHTML = '<i class="fa-solid fa-check"></i>';
            confirmButton.addEventListener("click", () => saveOreEdit(item, data));
            
            // Pulsante di annullamento modifica
            const cancelButton = document.createElement("button");
            cancelButton.className = "button-icon cancel option-button";
            cancelButton.title = "Annulla modifica";
            cancelButton.innerHTML = '<i class="fa-solid fa-times"></i>';
            cancelButton.addEventListener("click", () => cancelOreEdit(item));
            
            // Aggiunge gli elementi al container di modifica
            editContainer.appendChild(editInput);
            editContainer.appendChild(confirmButton);
            editContainer.appendChild(cancelButton);
            
            // Pulsante di modifica
            const editButton = document.createElement("button");
            editButton.className = "button-icon edit option-button";
            editButton.title = "Modifica Ore";
            editButton.id = `edit-button-${item.regOreId}`;
            editButton.innerHTML = '<i class="fa-solid fa-pencil"></i>';
            editButton.addEventListener("click", () => editOre(item));
            
            // Pulsante di eliminazione
            const deleteButton = document.createElement("button");
            deleteButton.className = "button-icon delete option-button";
            deleteButton.title = "Elimina Registrazione";
            deleteButton.innerHTML = '<i class="fa-solid fa-trash"></i>';
            deleteButton.addEventListener("click", () => deleteOre(item));
            
            // Aggiunge gli elementi al container delle azioni
            itemActions.appendChild(editContainer);
            itemActions.appendChild(editButton);
            itemActions.appendChild(deleteButton);
            li.appendChild(itemActions);
        }
        else
        {
            const logFilterObject = 
            {
                moid: item.moid,
                rtgStep: item.rtgStep,
                alternate: item.alternate,
                altRtgStep: item.altRtgStep,
                mono: item.mono,
                bom: item.bom,
                variant: item.variant,
                wc: item.wc,
                operation: item.operation,
                workerId: item.workerId,
                actionType: actionType
            }
            console.log("Log filter object:", logFilterObject); 
            const logList = await fetchLog(logFilterObject);

            const itemActions = document.createElement("div");
            itemActions.className = "item-actions";
            // Pulsante per l'apertura del Log
            const logButton = document.createElement("button");
            logButton.className = "button-icon log option-button";
            logButton.title = "Visualizza log per MoId: " + item.moid;
            logButton.id = `log-button-${item.invId}`;
            logButton.innerHTML = '<i class="fa-solid fa-list-ul"></i>';
            itemActions.appendChild(logButton);
            li.appendChild(itemActions);

            // Controlla se almeno un elemento nella lista interna "actionMessageDetails" ha messageType === "Errore"
            // Mostra icona di warning se ci sono errori nei log
            let hasError = false;

            // Controlla errori in actionMessageDetails
            if (
                Array.isArray(logList.actionMessageDetails) &&
                logList.actionMessageDetails.some(
                    detail =>
                        (typeof detail.actionStatus === "string" &&
                            (detail.actionStatus.includes("Errore") || detail.actionStatus.includes("Da Fare") || detail.actionStatus.includes("In Lavorazione") || detail.actionMessage !== ""))
                )
            ) {
                hasError = true;
            }

            // Controlla errori in omMessageDetails
            if (
                Array.isArray(logList.omMessageDetails) &&
                logList.omMessageDetails.some(
                    detail =>
                        (typeof detail.messageType === "string" &&
                            detail.messageType.includes("Errore"))
                )
            ) {
                hasError = true;
            }

            if (hasError) {
                const warningLogMessage = document.createElement("span");
                warningLogMessage.className = "log-message";
                warningLogMessage.title = "Log di errore rilevato";
                warningLogMessage.innerHTML = '<i class="fa-solid fa-triangle-exclamation"></i>';
                logButton.style.position = "relative";
                logButton.appendChild(warningLogMessage);
            }
            logButton.addEventListener("click", () => {
                openLogOverlay(logList);
            });
        }
        
        oreList.appendChild(li);
    }));

    if(paginationControls)
    {
        paginationControls.remove();
    }
    
    var pagination = createPagination("#ore-list");
    if(pagination)
    {
        pagination.updatePaginationControls();
    }

    if(data.length < 6)
    {
        const paginationContainer = document.querySelector('.pagination-controls');
        if (paginationContainer) {
            paginationContainer.classList.add('hidden');
        }
    }
}

// Funzione per gestire la modifica delle ore
function editOre(item) {
    // Identifica gli elementi DOM necessari
    const oreValueSpan = document.getElementById(`ore-value-${item.regOreId}`);
    const editContainer = document.getElementById(`edit-container-${item.regOreId}`);
    const editButton = document.getElementById(`edit-button-${item.regOreId}`);
    const editInput = document.getElementById(`edit-ore-input-${item.regOreId}`);
    
    // Nasconde il pulsante di modifica
    if (editButton) editButton.classList.add("hidden");
    
    // Mostra il container di modifica
    if (editContainer) editContainer.classList.remove("hidden");
    
    // Imposta il valore dell'input e metti il focus
    const convertedTime = (item.workingTime / 3600);
    if (editInput) {
        editInput.value = convertedTime;
        editInput.focus();
        editInput.select();
    }
}

// Funzione per salvare le modifiche alle ore
async function saveOreEdit(item, data) {
    // Ottiene il nuovo valore dall'input
    const editInput = document.getElementById(`edit-ore-input-${item.regOreId}`);
    const newWorkingTime = parseFloat(editInput.value);
    
    // Verifica che l'input sia valido
    if (isNaN(newWorkingTime) || newWorkingTime <= 0) {
        alert("Inserisci un valore numerico positivo.");
        return;
    }

    // Prepara i dati per l'aggiornamento
    const updateData = {
        regOreId: item.regOreId,
        workingTime: (newWorkingTime * 3600) // Converti in secondi
    };
    
    try {
        const response = await fetchWithAuth(`http://${getIPString()}:5245/api/reg_ore/view_ore/edit_working_time`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updateData)
        });
        
        if (!response.ok) {
            throw new Error("Errore durante l'aggiornamento delle ore");
        }
        
        // Aggiorna l'elemento nella lista
        item.workingTime = newWorkingTime * 3600; // Converti in secondi

        const dataItem = data.find(d => d.regOreId === item.regOreId);
        if (dataItem) {
            dataItem.workingTime = item.workingTime;
        }
        
        // Aggiorna la visualizzazione
        const oreValueSpan = document.getElementById(`ore-value-${item.regOreId}`);
        if (oreValueSpan) oreValueSpan.textContent = newWorkingTime;
        
        // Ripristina la visualizzazione normale
        cancelOreEdit(item);
        
        // Feedback all'utente
        oreValueSpan.classList.add("just-updated");
        setTimeout(() => {
            oreValueSpan.classList.remove("just-updated");
        }, 2000);

        populateOreList(data);
        
    } catch (error) {
        console.error("Errore durante l'aggiornamento:", error);
        alert("Si è verificato un errore durante l'aggiornamento delle ore.");
    }
}

// Funzione per annullare la modifica
function cancelOreEdit(item) {
    // Identifica gli elementi DOM necessari
    const editContainer = document.getElementById(`edit-container-${item.regOreId}`);
    const editButton = document.getElementById(`edit-button-${item.regOreId}`);
    
    // Mostra il pulsante di modifica
    if (editButton) editButton.classList.remove("hidden");
    
    // Nasconde il container di modifica
    if (editContainer) editContainer.classList.add("hidden");
}

// Funzione per gestire l'eliminazione delle ore
async function deleteOre(item) {
    const deleteData = {
        regOreId: item.regOreId
    }
    // Chiede conferma all'utente
    if (!confirm("Sei sicuro di voler eliminare questa registrazione?")) {
        return;
    }
    // Inserire body e non ID
    
    try {
        const response = await fetchWithAuth(`http://${getIPString()}:5245/api/reg_ore/view_ore/delete_reg_ore_id`, {
            method: "DELETE",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(deleteData)
        });
        
        if (!response.ok) {
            throw new Error("Errore durante l'eliminazione della registrazione");
        }
        
        // Rimuove l'elemento dalla lista filtrata
        filteredList = filteredList.filter(ore => ore.regOreId !== item.regOreId);
        
        // Aggiorna la vista
        populateOreList(filteredList);
        
        alert("Registrazione eliminata con successo!");
    } catch (error) {
        console.error("Errore durante l'eliminazione:", error);
        alert("Si è verificato un errore durante l'eliminazione della registrazione.");
    }
}   

async function openLogOverlay(logList) {
    console.log("MoId:", logList.moid);
    // Crea l'overlay se non esiste già
    let overlay = document.getElementById("log-overlay");
    if (!overlay) {
        overlay = document.createElement("div");
        overlay.id = "log-overlay";
        overlay.className = "log-overlay hidden";
        overlay.innerHTML = `
            <div class="log-overlay-content">
                <button id="close-log-overlay" class="close-log-overlay-btn" aria-label="Chiudi log">&times;</button>
                <div id="log-messages" class="log-messages"></div>
            </div>
        `;
        document.body.appendChild(overlay);
    }

    // Trova il messaggio con lo stesso moid
    const logMessagesDiv = overlay.querySelector("#log-messages");
    const logItem = logList;
    console.log("Log item:", logItem);

    if (logItem !== null && logItem.actionType !== null && logItem.moid !== null) {
        logMessagesDiv.innerHTML = ""; // Pulisci prima
        // Mostra MoId e MoNo
        logMessagesDiv.innerHTML += `<div class="msg-header"><strong>WorkerId:</strong> ${logItem.workerId}</div>
        <div class="msg-header"><strong>MoId:</strong> ${logItem.moid}</div>
        <div class="msg-header"><strong>MoNo:</strong> ${logItem.mono}</div>`;

        const messageDateTime = parseDateTime(logItem.messageDate);
        // Prima mostra gli errori di omMessageDetails
        if(Array.isArray(logItem.omMessageDetails) && logItem.omMessageDetails && logItem.omMessageDetails.length > 0) {
            logItem.omMessageDetails.forEach(msg => {
                if(msg.messageId)
                {
                    logMessagesDiv.innerHTML += `
                    <div style="margin-bottom:10px;">
                        <strong class="msg-id"><u>Messaggio #${msg.messageId}:</u></strong> <br>
                        <div class="msg-content">
                            <strong>Tipo Messaggio:</strong> ${msg.messageType} <br>
                            <strong>Messaggio:</strong> ${msg.messageText} <br>
                            <strong>Data:</strong> ${messageDateTime.parsedDate} <br>
                        </div>
                    </div>
                    `;
                }
            });
        }

        // Se actionMessageDetails è un array, mostra ogni messaggio come JSON
        if (Array.isArray(logItem.actionMessageDetails) && logItem.actionMessageDetails.length > 0) {
            logItem.actionMessageDetails.forEach(msg => {
                // const dateTime = parseDateTime(msg.messageDate);
                // const createdDateTime = parseDateTime(msg.tbcreated);
                // const editedDateTime = parseDateTime(msg.tbmodified);
                logMessagesDiv.innerHTML += `
                <div style="margin-bottom:10px;">
                    <strong class="msg-id"><u>Messaggio #${msg.actionId}:</u></strong> <br>
                    <div class="msg-content">
                        <strong>Stato Azione:</strong> ${msg.actionStatus} <br>
                        ${msg.actionMessage !== null ? "<strong>Messaggio:</strong>" + msg.actionMessage + "<br>" : "" } 
                        <strong>Stato chiusura:</strong> ${(msg.closed === true || msg.closed === 1) ? "Chiuso" : "Aperto"} <br>
                        <strong>Tipo Specificazione:</strong> ${msg.specificatorType} <br>
                        <strong>Mo Status:</strong> ${msg.mostatus} <br>
                    </div>
                </div>
                `;
            });
        } else {
            logMessagesDiv.innerHTML += `<div class="msg-header"><strong>Attenzione!</strong></div>
                <div><strong>Messaggio:</strong> Nessun messaggio disponibile. L'operazione potrebbe non essere disponibile o essere stata chiusa</div>`;
        }
    } else {
        logMessagesDiv.innerHTML = `<div class="msg-header"><strong>Attenzione!</strong></div>
                <div><strong>Messaggio:</strong> Nessun messaggio disponibile. L'operazione potrebbe non essere disponibile o essere stata chiusa</div>`;
    }

    // Mostra l'overlay
    overlay.classList.remove("hidden");

    // Gestione chiusura overlay
    const closeBtn = overlay.querySelector("#close-log-overlay");
    closeBtn.onclick = () => {
        overlay.classList.add("hidden");
    };
}

async function fetchLog(filterLIst) {
    const data = 
    { 
        moid: filterLIst.moid,
        rtgStep: filterLIst.rtgStep,
        alternate: filterLIst.alternate,
        altRtgStep: filterLIst.altRtgStep,
        mono: filterLIst.mono,
        bom: filterLIst.bom,
        variant: filterLIst.variant,
        wc: filterLIst.wc,
        operation: filterLIst.operation,
        workerId: filterLIst.workerId,
        actionType: filterLIst.actionType
    };

    try {
        console.log("MoId da cercare:", data);
        const response = await fetchWithAuth(`http://${getIPString()}:5245/api/action_message/get_action_messages_filtered`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        });
        if (!response.ok) {
            throw new Error("Errore nella risposta del server");
        }
        const logData = await response.json();
        return logData;
    } catch (error) {
        console.error("Errore durante il recupero dei log:", error);
    }
}