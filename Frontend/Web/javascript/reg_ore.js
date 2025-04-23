import { fetchWithAuth } from "./fetch.js";
import { getCookie } from "./cookies.js";
import { setupAutocomplete } from "./autocomplete.js";

document.addEventListener("DOMContentLoaded", async function () {
    // Recupera elementi DOM
    const commessaInput = document.getElementById("reg-ore-commessa");
    const commessaAutocompleteList = document.getElementById("reg-ore-commessa-autocomplete-list");
    const lavorazioneInput = document.getElementById("reg-ore-lavorazione");
    const lavorazioneAutocompleteList = document.getElementById("reg-ore-lavorazione-autocomplete-list");
    const odlInput = document.getElementById("reg-ore-odp");
    const odlAutocompleteList = document.getElementById("reg-ore-odp-autocomplete-list");
    const addButton = document.getElementById("inv-add-list");
    const cercaButton = document.getElementById("reg-ore-cerca");
    const searchOverlay = document.getElementById("search-overlay");
    const closeSearchButton = document.getElementById("close-search-overlay");
    const cancelSearchButton = document.getElementById("cancel-search");
    const selectSearchResultButton = document.getElementById("select-search-result");
    const searchResultsBody = document.getElementById("search-results-body");
    const oreInput = document.getElementById("reg-ore-ore");
    const saveButton = document.getElementById("inv-save");
    const noContent = document.getElementById("nocontent");

    // Liste di dati
    let jobList = [];
    let odpList = [];
    let lavorazioneList = [];
    let searchResults = [];
    let selectedSearchRow = null;
    let dataResultList = [];

    // Inizializza l'autocompletamento per la commessa e carica i dati iniziali
    try {
        const jobResult = await fetchAllJobs();
        jobList = jobResult
            .filter(job => job && job.job && job.description)
            .map(job => ({
                job: job.job,
                description: job.description,
                display: `${job.job} - ${job.description}`
            }));
        // console.log("Lista di lavori:", jobList);
        setupAutocomplete(commessaInput, commessaAutocompleteList, jobList);
    } catch (error) {
        console.error("Errore nel caricamento iniziale dei lavori:", error);
    }

    // Event listener per il cambio di commessa
    commessaInput.addEventListener("change", async function() {
        // Resetta i campi dipendenti Ordine di Lavoro e Lavorazione
        odlInput.value = "";
        lavorazioneInput.value = "";
        odpList = [];
        lavorazioneList = [];
        
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        if (selectedCommessa) {
            await loadOdpData(selectedCommessa.job);
        }
    });

    odlInput.addEventListener("change", async function() {
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
    
        // Resetta il campo lavorazione se i dati non sono validi
        if (!selectedCommessa || !selectedOdp) {
            lavorazioneInput.value = ""; 
            lavorazioneList = [];
            return;
        }
    
        // Carica i dati della lavorazione solo se necessario
        // Serve in caso di selezione dalla tabella in overlay
        await loadLavorazioneData(selectedCommessa.job, selectedOdp.odp, selectedOdp.creationDate);
    });

    // Event listener per il cambio di lavorazione
    lavorazioneInput.addEventListener("change", async function() {
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
        if (selectedCommessa && selectedOdp && selectedLavorazione) {
            console.log("Commessa selezionata:", selectedCommessa);
            console.log("ODP selezionato:", selectedOdp);
            console.log("Lavorazione selezionata:", selectedLavorazione);
            oreInput.focus
            await loadAllData(selectedCommessa.job, selectedOdp.odp, selectedOdp.creationDate, selectedLavorazione.operation);
        }
    });
        

    // Event listener per il pulsante Cerca - Ora apre la tabella overlay
    cercaButton.addEventListener("click", async function() {
        // Filtra i risultati in base al valore nel campo commessa
        let filteredResults = [];
        
        if (commessaInput) {
            // Se c'è un valore nel campo commessa, filtra i risultati
            const jobSearchResults = jobList.filter(job => 
                job.display.toLowerCase().includes(commessaInput.value.toLowerCase())
            );

            for(const job of jobSearchResults) {
                const results = await fetchJobMostep(job.job);
                console.log("Risultati della ricerca:", results);
                filteredResults.push(...results.map(item => ({
                    job: job.job,
                    mono: item.mono || '',
                    creationDate: item.creationDate || '',
                    um: item.um || '',
                    resQty: item.resQty || '',
                    bom: item.bom || '',
                    itemDesc: item.itemDesc || '',
                    operation: item.operation || '',
                    operDesc: item.operDesc || ''
                })));
            }
        } else {
            // Se il campo commessa è vuoto, mostra tutti i lavori disponibili con struttura semplificata
            filteredResults = jobList.map(job => ({
                job: job.job,
                description: job.description,
                mono: '',
                creationDate: '',
                um: '',
                resQty: '',
                bom: '',
                itemDesc: job.description || '',
                operation: '',
                operDesc: ''
            }));
        }
        
        searchResults = filteredResults;
        populateSearchResults(searchResults);
        searchOverlay.classList.add("active");
    });

    // Event listeners per i pulsanti di chiusura dell'overlay
    closeSearchButton.addEventListener("click", function() {
        searchOverlay.classList.remove("active");
        selectedSearchRow = null;
    });

    cancelSearchButton.addEventListener("click", function() {
        searchOverlay.classList.remove("active");
        selectedSearchRow = null;
    });

    // Event listener per il pulsante Seleziona
    selectSearchResultButton.addEventListener("click", function() {
        if (selectedSearchRow) {
            // Cerca il job corrispondente nella lista originale per ottenere la description
            const jobItem = jobList.find(job => job.job === selectedSearchRow.job);
            const description = jobItem ? jobItem.description : selectedSearchRow.itemDesc;
            
            // Compila il campo commessa
            commessaInput.value = `${selectedSearchRow.job} - ${description}`;
            
            const commessaEvent = new Event('change', { bubbles: true });
            commessaInput.dispatchEvent(commessaEvent);

            // Compila il campo ODP/mono
            if (selectedSearchRow.mono) {
                odlInput.value = `${selectedSearchRow.mono} - ${selectedSearchRow.creationDate}`;
            } else {
                odlInput.value = ""; // Resetta il campo se non c'è un mono
            }

            if (odlInput.value) {
                setTimeout(() => {
                    const odpEvent = new Event('change', { bubbles: true });
                    odlInput.dispatchEvent(odpEvent);
                    console.log("Evento change per ODP dispatched");
                }, 300); // Piccolo ritardo per permettere al primo evento di completarsi
            }

            
            // Compila il campo lavorazione
            if (selectedSearchRow.operation) {
                lavorazioneInput.value = `${selectedSearchRow.operation} - ${selectedSearchRow.operDesc}`;
            } else {
                lavorazioneInput.value = ""; // Resetta il campo se non c'è un'operazione
            }

            // Trigger per lavorazione
            if (lavorazioneInput.value) {
                setTimeout(() => {
                    const lavorazioneEvent = new Event('change', { bubbles: true });
                    lavorazioneInput.dispatchEvent(lavorazioneEvent);
                    console.log("Evento change per lavorazione dispatched");
                }, 600);
            }


            // Chiude l'overlay
            searchOverlay.classList.remove("active");
            
            // Azzera la selezione alla fine
            selectedSearchRow = null;
        } else {
            alert("Seleziona una riga prima di procedere");
        }
    });

    // Funzione per popolare la tabella di risultati
    function populateSearchResults(results) {
        searchResultsBody.innerHTML = "";
        selectedSearchRow = null;
        
        if (results.length === 0) {
            const row = searchResultsBody.insertRow();
            const cell = row.insertCell();
            cell.colSpan = 9;
            cell.textContent = "Nessun risultato trovato";
            return;
        }
        
        results.forEach((result, index) => {
            const row = searchResultsBody.insertRow();
            row.dataset.index = index;
            
            // Aggiunge celle con i dati
            const cellJob = row.insertCell();
            cellJob.textContent = result.job;
            
            const cellMono = row.insertCell();
            cellMono.textContent = result.mono;
            
            const cellCreationDate = row.insertCell();
            cellCreationDate.textContent = result.creationDate;
            
            const cellUM = row.insertCell();
            cellUM.textContent = result.um;
            
            const cellResQty = row.insertCell();
            cellResQty.textContent = result.resQty;
            
            const cellBOM = row.insertCell();
            cellBOM.textContent = result.bom;
            
            const cellItemDesc = row.insertCell();
            cellItemDesc.textContent = result.itemDesc;
            
            const cellOperation = row.insertCell();
            cellOperation.textContent = result.operation;
            
            const cellOperDesc = row.insertCell();
            cellOperDesc.textContent = result.operDesc;

            
            
            // Event listener per la selezione della riga
            row.addEventListener("click", function() {
                // Rimuovi la selezione precedente
                document.querySelectorAll("#search-results-body tr.selected").forEach(tr => {
                    tr.classList.remove("selected");
                });
                
                // Seleziona questa riga
                row.classList.add("selected");
                selectedSearchRow = results[index];
            });
            
            // Doppio click per selezionare e confermare
            row.addEventListener("dblclick", function() {
                selectedSearchRow = results[index];
                selectSearchResultButton.click(); 
            });
        });
    }

    // SaveButton: chiamata all'API passando dataResultList per salvare i dati.
    // Chiama la rimozione di tutti gli elementi dalla lista temporanea
    saveButton.addEventListener("click", async function() {
        if (dataResultList.length > 0) {
            // console.log("Dati da salvare:", dataResultList);
            // Recupera il workerid dai cookies
            const cookie = JSON.parse(getCookie("userInfo"));
            console.log(typeof(cookie));
            console.log("Cookie:", cookie);
            const workerId = cookie.workerId.toString();
            console.log("Worker ID:", workerId);
            if (!workerId) {
                console.error("Worker ID non trovato nei cookie.");
                return;
            }
            // Aggiunge il workerId a ogni oggetto nella lista
            dataResultList.forEach(item => {
                item.workerId = workerId;
                item.workingTime = (item.workingTime * 3600); // Converte ore in secondi
            });
            // console.log("Lista con Worker ID:", dataResultList);
            // console.log("Tipo Lista con Worker ID:", typeof(dataResultList));
            // console.log("Tipo lista convertita: ", typeof(JSON.stringify(dataResultList)));
            try {
                const response = await fetchWithAuth("http://localhost:5245/api/reg_ore/post_reg_ore", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(dataResultList),
                });
                if (response.ok) {
                    const result = await response.json();
                    console.log("Dati salvati con successo:", result);
                    // Pulisce la lista temporanea
                    const list = document.getElementById("reg-ore-lista-temp");
                    while (list.firstChild) {
                        list.removeChild(list.firstChild);
                    }
                    dataResultList = []; // Resetta la lista dei risultati
                    commessaInput.value =  "";
                    odlInput.value = "";
                    lavorazioneInput.value = "";
                    oreInput.value = "";
                    noContent.classList.remove("hidden");
                    alert("Dati salvati con successo!");
                } else {
                    console.error("Errore durante il salvataggio dei dati:", response.status, response.statusText);
                }
            } catch (error) {
                console.error("Errore durante la richiesta di salvataggio:", error);
            }
        } else {
            alert("Nessun dato da salvare. Aggiungi prima un elemento.");
        }
    }); 

    // Event listener per il pulsante Aggiungi
    addButton.addEventListener("click", async function() {
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
        const ore = document.getElementById("reg-ore-ore").value;
    
        console.log("Dati selezionati:", {
            selectedCommessa,
            selectedOdp,
            selectedLavorazione,
            ore
        });
    
        if (selectedCommessa && selectedOdp && selectedLavorazione && ore) {
            const result = await loadAllData(selectedCommessa.job, selectedOdp.odp, selectedOdp.creationDate, selectedLavorazione.operation);
            // console.log("Risultato di loadAllData:", result);
            if (result) {
                var data = {
                    job: result.job,
                    rtgStep: result.rtgStep,
                    alternate: result.alternate,
                    altRtgStep: result.altRtgStep,
                    operation: result.operation,
                    operDesc: result.operDesc,
                    bom: result.bom,
                    variant: result.variant,
                    itemDesc: result.itemDesc,
                    moid: result.moid,
                    mono: result.mono,
                    creationDate: result.creationDate,
                    uom: result.uom,
                    productionQty: result.productionQty,
                    producedQty: result.producedQty,
                    resQty: result.resQty,
                    storage: result.storage,
                    workingTime: ore,
                    wc: result.wc,
                }
                dataResultList.push(data);
                // console.log("Lista di risultati:", dataResultList);
                addToTemporaryList(data, dataResultList);
                // Reset campo ore
                commessaInput.value = "";
                odlInput.value = "";
                lavorazioneInput.value = "";
                oreInput.value = "";
            } else {
                alert("Errore: impossibile aggiungere l'elemento. Dati mancanti o non validi.");
            }
        } else {
            alert("Compilare tutti i campi richiesti");
        }
    });

    // Funzione per trovare l'elemento selezionato in base al valore dell'input
    // Si può esportare
    function findSelectedItem(inputValue, list) {
        if (!inputValue || !list || list.length === 0) return null;
        
        return list.find(item => item.display === inputValue);
    }

    // Funzione per caricare i dati dell'ODP in base alla commessa selezionata
    async function loadOdpData(jobId) {
        if (!jobId) return;
        
        try {
            const odpResult = await fetchJobMostep(jobId);
            odpList = odpResult
                .filter(odp => odp && odp.mono && odp.creationDate)
                .map(odp => ({
                    odp: odp.mono,
                    creationDate: odp.creationDate,
                    display: `${odp.mono} - ${odp.creationDate}`
                }));

            console.log("Lista di ODP:", odpList);
            setupAutocomplete(odlInput, odlAutocompleteList, odpList);
        } catch (error) {
            console.error("Errore nel caricamento dei dati ODP:", error);
        }
    }

    // Funzione per caricare i dati della lavorazione
    async function loadLavorazioneData(jobId, mono, creationDate) {
        if (!jobId || !mono || !creationDate) return;
        
        try {
            const lavorazioneResult = await fetchJobsByOdp(jobId, mono, creationDate);
            lavorazioneList = lavorazioneResult
                .filter(lav => lav && lav.operation && lav.operDesc)
                .map(lav => ({
                    operation: lav.operation,
                    operDesc: lav.operDesc,
                    display: `${lav.operation} - ${lav.operDesc}`
                }));

            console.log("Lista di lavorazioni:", lavorazioneList);
            setupAutocomplete(lavorazioneInput, lavorazioneAutocompleteList, lavorazioneList);
        } catch (error) {
            console.error("Errore nel caricamento dei dati lavorazione:", error);
        }
    }

    async function loadAllData(jobId, mono, creationDate, operation) {
        if (!jobId || !mono || !creationDate || !operation) return null;
        
        try {
            const allDataResult = await fetchJobsByLavorazione(jobId, mono, creationDate, operation);
            // console.log("Lista di tutti i dati:", allDataResult);
    
            // Restituisce il primo elemento o un oggetto vuoto
            return allDataResult.length > 0 ? allDataResult[0] : null;
        } catch (error) {
            console.error("Errore nel caricamento dei dati:", error);
            return null;
        }
    }
});

// Funzioni di fetch
async function fetchJobMostep(job) {
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/mostep/job", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"job": job}),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const jobInfo = await request.json();
        return jobInfo;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchJobsByOdp(job, mono, creationDate) {
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/mostep/odp", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate
            }),
        });
        
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        
        const jobInfo = await request.json();
        return jobInfo;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchAllJobs() {
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/job", {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const jobs = await request.json();
        return jobs;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
}

async function fetchJobsByLavorazione(job, mono, creationDate, operation) {
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/mostep/lavorazioni", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate,
                "operation": operation
            }),
        });
        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }
        const data = await request.json();
        return data;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
    }
}

// Si può esportare a patto di mantenere gli id invariati nel file html
function addToTemporaryList(data, dataResultList) {
    const list = document.getElementById("reg-ore-lista-temp");
    const noContent = document.getElementById("nocontent");
    const newItem = document.createElement("li");
    newItem.classList.add("just-added"); // Aggiungi classe per l'animazione

    newItem.innerHTML = `
        <div class="item-content"><p>${data.job} / ${data.moid} - ${data.mono} / ${data.operation} / ${data.operDesc}</p><strong>${data.workingTime} ore</strong></div>
        <div class="item-actions">
            <button class="button-icon delete option-button" title="Rimuovi">
                <i class="fa-solid fa-trash"></i>
            </button>
        </div>
    `;

    list.appendChild(newItem);

    if(list.childElementCount > 0) {
        noContent.classList.add("hidden");
    }

    // Aggiunge event listener per il pulsante di eliminazione
    const deleteButton = newItem.querySelector(".delete");
    deleteButton.addEventListener("click", function() {
        dataResultList.splice(dataResultList.indexOf(data), 1);
        list.removeChild(newItem);
        if(list.childElementCount > 0) {
            noContent.classList.add("hidden");
        }
        else if (list.childElementCount === 0) 
        {
            noContent.classList.remove("hidden");
        }
    });
}