import { fetchWithAuth } from "./fetch.js";
import { getCookie } from "./cookies.js";
import { setupAutocomplete } from "./autocomplete.js";
import { getIPString } from "./main.js";

let globalAllData = null;

document.addEventListener("DOMContentLoaded", async function () {
    // Recupera elementi DOM
    const commessaInput = document.getElementById("prel-mat-commessa");
    const commessaAutocompleteList = document.getElementById("prel-mat-commessa-autocomplete-list");
    const lavorazioneInput = document.getElementById("prel-mat-lavorazione");
    const lavorazioneAutocompleteList = document.getElementById("prel-mat-lavorazione-autocomplete-list");
    const odlInput = document.getElementById("prel-mat-odp");
    const odlAutocompleteList = document.getElementById("prel-mat-odp-autocomplete-list");
    const barcodeInput = document.getElementById("prel-mat-barcode");
    const barcodeAutocompleteList = document.getElementById("prel-mat-barcode-autocomplete-list");
    const addButton = document.getElementById("inv-add-list");
    const cercaButton = document.getElementById("prel-mat-cerca");
    const searchOverlay = document.getElementById("search-overlay");
    const closeSearchButton = document.getElementById("close-search-overlay");
    const cancelSearchButton = document.getElementById("cancel-search");
    const selectSearchResultButton = document.getElementById("select-search-result");
    const searchResultsBody = document.getElementById("search-results-body");
    const quantitaInput = document.getElementById("prel-mat-quantita");
    const saveButton = document.getElementById("inv-save");
    const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
    // Aggiunto quando viene salvata la lista viene
    const noContent = document.getElementById("nocontent");
    const errorQty = document.getElementById("error-quantity");

    // Liste di dati
    let jobList = [];
    let odpList = [];
    let lavorazioneList = [];
    let barcodeList = [];
    let searchResults = [];
    let selectedSearchRow = null;
    let dataResultList = [];

    let isFillingFromOverlay = false;

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
        alert("Qualcosa è andato storto durante il caricamento iniziale dei dati.");
    }

    // Event listener per il cambio di commessa
    commessaInput.addEventListener("change", async function() {
        // Resetta i campi dipendenti Ordine di Lavoro e Lavorazione
        
        if (isFillingFromOverlay)
        {
            odlInput.value = "";
            odlInput.disabled = true;
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
        
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        if (selectedCommessa) {
            await loadOdpData(selectedCommessa.job);
        }
        else {
            odlInput.value = "";
            odlInput.disabled = true;
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    });

    commessaInput.addEventListener("focusout", function() {
        // Resetta i campi successivi se il campo commessa è vuoto
        if (commessaInput.value === "") {
            odlInput.value = "";
            odlInput.disabled = true;
            odlInput.value = "";
            odlAutocompleteList.innerHTML = "";
            odlAutocompleteList.classList.add("hidden");
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            lavorazioneList = [];
            lavorazioneAutocompleteList.innerHTML = ""; 
            lavorazioneAutocompleteList.classList.add("hidden");
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = "";
            barcodeAutocompleteList.classList.add("hidden"); 
            quantitaInput.disabled = true; 
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    odlInput.addEventListener("change", async function() {
    
        if (!isFillingFromOverlay)
        {
            // Resetta il campo lavorazione se i dati non sono validi
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true; 
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    
        // Carica i dati della lavorazione solo se necessario
        // Serve in caso di selezione dalla tabella in overlay
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        if (selectedCommessa && selectedOdp) {
            await loadLavorazioneData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate);
        }
        else {
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true; 
            lavorazioneList = [];
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            barcodeList = [];
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }
    });

    odlInput.addEventListener("focusout", function() {
        // Resetta i campi successivi se il campo ordine di produzione è vuoto
        if (odlInput.value === "") {
            lavorazioneInput.value = "";
            lavorazioneInput.disabled = true;
            lavorazioneList = [];
            lavorazioneAutocompleteList.innerHTML = ""; 
            lavorazioneAutocompleteList.classList.add("hidden");
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = "";
            barcodeAutocompleteList.classList.add("hidden"); 
            quantitaInput.disabled = true; 
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    // Event listener per il cambio di lavorazione
    lavorazioneInput.addEventListener("change", async function() {
    
        if (!isFillingFromOverlay)
        {
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            quantitaInput.disabled = true;
            quantitaInput.value = "";
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
        }

        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);

        if (selectedCommessa && selectedOdp && selectedLavorazione) {
            await loadBarCodeData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation);
            console.log("Lista barcode:", barcodeList);
        }
    });

    lavorazioneInput.addEventListener("focusout", function() {
        // Resetta il campo barcode se il campo lavorazione è vuoto
        if (lavorazioneInput.value === "") {
            barcodeInput.value = "";
            barcodeInput.disabled = true;
            quantitaInput.value = "";
            barcodeList = [];
            barcodeAutocompleteList.innerHTML = ""; // Svuota la lista di autocompletamento
            barcodeAutocompleteList.classList.add("hidden"); // Nasconde la lista di autocompletamento
            quantitaInput.disabled = true; // Disabilita il campo quantità
            errorQty.style.display = "none";
            if (quantitaLabel) {
                quantitaLabel.textContent = "Quantità: ";
            }
            return;
        }
    });

    // Event listener per il cambio di barcode
    barcodeInput.addEventListener("change", async function() {
        const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
        const selectedOdp = findSelectedItem(odlInput.value, odpList);
        const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
        const selectedBarcode = findSelectedItem(barcodeInput.value, barcodeList);
        if (selectedCommessa && selectedOdp && selectedLavorazione && selectedBarcode) {
            console.log("Commessa selezionata:", selectedCommessa);
            console.log("ODP selezionato:", selectedOdp);
            console.log("Lavorazione selezionata:", selectedLavorazione);
            console.log("Barcode selezionato:", selectedBarcode);
            await loadAllData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation, selectedBarcode.barCode);
            quantitaInput.disabled = false;
            quantitaInput.focus();
        }
    });

    barcodeInput.addEventListener("focusout", function() {
        if (barcodeInput.value === "") {
            quantitaInput.disabled = true; 
        }
    });

    barcodeInput.addEventListener("keydown", async function(event) {
        if (event.key === "Enter") { 
            event.preventDefault(); 
    
            const selectedCommessa = findSelectedItem(commessaInput.value, jobList);
            const selectedOdp = findSelectedItem(odlInput.value, odpList);
            const selectedLavorazione = findSelectedItem(lavorazioneInput.value, lavorazioneList);
            const selectedBarcode = barcodeList.find(item => item.barCode === barcodeInput.value.toUpperCase());
            barcodeInput.value = selectedBarcode ? selectedBarcode.display : "";
    
            if (selectedCommessa && selectedOdp && selectedLavorazione && selectedBarcode) {
                console.log("Commessa selezionata:", selectedCommessa);
                console.log("ODP selezionato:", selectedOdp);
                console.log("Lavorazione selezionata:", selectedLavorazione);
                console.log("Barcode selezionato:", selectedBarcode);
                await loadAllData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation, selectedBarcode.barCode);
                barcodeAutocompleteList.classList.add("hidden");
                quantitaInput.focus();
            }
        }
    });

    quantitaInput.addEventListener("keydown", function(event) {
        if (event.key === "Enter") {
            event.preventDefault();
            addButton.click();
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
                //console.log("Risultati della ricerca:", results);
                filteredResults.push(...results.map(item => ({
                    job: job.job,
                    mono: item.mono || '',
                    creationDate: item.creationDate || '',
                    prelUoM: item.prelUoM || '',
                    prelResQty: item.prelResQty || '',
                    bom: item.bom || '',
                    itemDesc: item.itemDesc || '',
                    operation: item.operation || '',
                    operDesc: item.operDesc || '',
                    component: item.component || '',
                    position: item.position || '',
                    barCode: item.barCode || ''
                })));
            }
        } else {
            for(const job of jobList) {
                const results = await fetchJobMostep(job.job);
                //console.log("Risultati della ricerca:", results);
                filteredResults.push(...results.map(item => ({
                    job: job.job,
                    mono: item.mono || '',
                    creationDate: item.creationDate || '',
                    prelUoM: item.prelUoM || '',
                    prelResQty: item.prelResQty || '',
                    bom: item.bom || '',
                    itemDesc: item.itemDesc || '',
                    operation: item.operation || '',
                    operDesc: item.operDesc || '',
                    component: item.component || '',
                    position: item.position || '',
                    barCode: item.barCode || ''
                })));
            }
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
            
            isFillingFromOverlay = true; // Imposta il flag per evitare conflitti con gli eventi di input

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

            if(odlInput.value) {
                setTimeout(() => {
                    const odpEvent = new Event('change', { bubbles: true });
                    odlInput.dispatchEvent(odpEvent);
                    //console.log("Evento change per ODP dispatched");
                }, 300); // Piccolo ritardo per permettere al primo evento di completarsi
            }
            
            // Compila il campo lavorazione
            if (selectedSearchRow.operation) {
                console.log("Lavorazione trovata in overlay:", selectedSearchRow.operation);
                lavorazioneInput.value = `${selectedSearchRow.operation} - ${selectedSearchRow.operDesc}`;
            } else {
                lavorazioneInput.value = ""; // Resetta il campo se non c'è un'operazione
            }

            if(lavorazioneInput.value) {
                setTimeout(() => {
                    const lavorazioneEvent = new Event('change', { bubbles: true });
                    lavorazioneInput.dispatchEvent(lavorazioneEvent);
                    //console.log("Evento change per lavorazione dispatched");
                }, 600);
            }

            if(selectedSearchRow.barCode) {
                console.log("Barcode trovato in overlay:", selectedSearchRow.barCode);
                barcodeInput.value = `Item: ${selectedSearchRow.component} - Code: ${selectedSearchRow.barCode} - ${selectedSearchRow.itemDesc}`;
            }
            else {
                barcodeInput.value = ""; // Resetta il campo se non c'è un barcode
            }

            if(barcodeInput.value) {
                setTimeout(() => {
                    const barcodeEvent = new Event('change', { bubbles: true });
                    barcodeInput.dispatchEvent(barcodeEvent);
                    //console.log("Evento change per barcode dispatched");
                }, 900);
            }

            setTimeout(() => {
                console.log("isFillingFromOverlay:", isFillingFromOverlay);
                isFillingFromOverlay = false; // Ripristina il flag
            }, 1000);

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
            cell.colSpan = 12;
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
            cellUM.textContent = result.prelUoM;
            
            const cellResQty = row.insertCell();
            cellResQty.textContent = result.prelResQty;
            
            const cellBOM = row.insertCell();
            cellBOM.textContent = result.bom;
            
            const cellItemDesc = row.insertCell();
            cellItemDesc.textContent = result.itemDesc;
            
            const cellOperation = row.insertCell();
            cellOperation.textContent = result.operation;
            
            const cellOperDesc = row.insertCell();
            cellOperDesc.textContent = result.operDesc;

            const cellComponent = row.insertCell();
            cellComponent.textContent = result.component;

            const cellPosition = row.insertCell();
            cellPosition.textContent = result.position;       
            
            const cellBarCode = row.insertCell();
            cellBarCode.textContent = result.barCode;
            
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
            console.log("Dati da salvare:", dataResultList);
            var workerId = "";
            const puCookie = JSON.parse(getCookie("pu-User"));
            if(puCookie) {
                //console.log("cookie pu-User:", puCookie);
                workerId = puCookie.workerId.toString();
                console.log("L'operazione viene salvata con l'utente:", workerId);
            }
            else {
                // Recupera il workerid dai cookies
                const cookie = JSON.parse(getCookie("userInfo"));
                console.log(typeof(cookie));
                //console.log("Cookie:", cookie);
                workerId = cookie.workerId.toString();
                console.log("Worker ID:", workerId);
            }

            if (!workerId || workerId === "") {
                console.error("Worker ID non trovato nei cookie.");
                return;
            }
            // Aggiunge il workerId a ogni oggetto nella lista
            dataResultList.forEach(item => {
                item.workerId = workerId;
            });
            console.log("Lista con Worker ID:", dataResultList);

            try {
                const response = await fetchWithAuth(`http://${getIPString()}:5245/api/prel_mat/post_prel_mat`, {
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
                    const list = document.getElementById("prel-mat-lista-temp");
                    while (list.firstChild) {
                        list.removeChild(list.firstChild);
                    }
                    dataResultList = []; // Resetta la lista dei risultati
                    commessaInput.value =  "";
                    odlInput.value = "";
                    odlInput.disabled = true;
                    lavorazioneInput.value = "";
                    lavorazioneInput.disabled = true;
                    barcodeInput.value = "";
                    barcodeInput.disabled = true;
                    quantitaInput.value = "";
                    quantitaInput.disabled = true;
                    noContent.classList.remove("hidden");
                    alert("Dati salvati con successo");
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
        const selectedBarcode = findSelectedItem(barcodeInput.value, barcodeList);
        const selectedQta = document.getElementById("prel-mat-quantita").value;
    
        console.log("Dati selezionati:", {
            selectedCommessa,
            selectedOdp,
            selectedLavorazione,
            selectedBarcode,
            selectedQta
        });
    
        if (selectedCommessa && selectedOdp && selectedLavorazione && selectedBarcode && selectedQta) {
            const result = await loadAllData(selectedCommessa.job, selectedOdp.mono, selectedOdp.creationDate, selectedLavorazione.operation, selectedBarcode.barCode);
            //console.log("Risultato di loadAllData:", result);
            if (result) {
                var data = {
                    job: result.job,
                    rtgStep: result.rtgStep,
                    alternate: result.alternate,
                    altRtgStep: result.altRtgStep,
                    operation: result.operation,
                    operDesc: result.operDesc,
                    position: result.position,
                    component: result.component,
                    bom: result.bom,
                    variant: result.variant,
                    itemDesc: result.itemDesc,
                    moid: result.moid,
                    mono: result.mono,
                    creationDate: result.creationDate,
                    uoM: result.uoM,
                    productionQty: result.productionQty,
                    producedQty: result.producedQty,
                    resQty: result.resQty,
                    storage: result.storage,
                    barCode: result.barCode,
                    wc: result.wc,
                    prelQty: selectedQta
                }
                dataResultList.push(data);

                if(selectedQta > result.finalSum) 
                {
                    errorQty.style.display = "block";
                }

                //console.log("Lista di risultati:", dataResultList);
                addToTemporaryList(data, dataResultList);
                // Reset campo quantità
                commessaInput.value = "";
                odlInput.value = "";
                odlInput.disabled = true;
                lavorazioneInput.value = "";
                lavorazioneInput.disabled = true;
                barcodeInput.value = "";
                barcodeInput.disabled = true;
                quantitaInput.value = "";
                quantitaInput.disabled = true;
                const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
                if (quantitaLabel) {
                    quantitaLabel.textContent = "Quantità: ";
                }
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
            odlInput.disabled = false;
            const odpResult = await fetchJobMostep(jobId);
            //console.log("Risultato ODP:", odpResult);
            odpList = odpResult
                .filter(odp => odp && odp.mono && odp.creationDate)
                .map(odp => ({
                    mono: odp.mono,
                    creationDate: odp.creationDate,
                    display: `${odp.mono} - ${odp.creationDate}`
                }));

            //console.log("Lista di ODP:", odpList);
            setupAutocomplete(odlInput, odlAutocompleteList, odpList);
            const odpDistinctList = odpList.filter((item, index, self) =>
                index === self.findIndex((t) => t.odp === item.odp && t.creationDate === item.creationDate));
            //console.log("Lista di ODP Distinti:", odpDistinctList);
            if(odpDistinctList.length === 1) {
                setTimeout(() => {
                    odlInput.value = odpDistinctList[0].display;
                    const event = new Event('change', { bubbles: true });
                    odlInput.dispatchEvent(event);
                    odlInput.disabled = true;
                }, 100);
            }
        } catch (error) {
            console.error("Errore nel caricamento dei dati ODP:", error);
        }
    }

    // Funzione per caricare i dati della lavorazione
    async function loadLavorazioneData(jobId, mono, creationDate) {
        console.log("Caricamento lavorazione per jobId:", jobId, "e mono:", mono);
        if (!jobId || !mono || !creationDate) return;
        
        try {
            lavorazioneInput.disabled = false;
            const lavorazioneResult = await fetchJobsByOdp(jobId, mono, creationDate);
            //console.log("Risultato lavorazione:", lavorazioneResult);
            lavorazioneList = lavorazioneResult
                .filter(lav => lav && lav.operation && lav.operDesc)
                .map(lav => ({
                    operation: lav.operation,
                    operDesc: lav.operDesc,
                    display: `${lav.operation} - ${lav.operDesc}`
                }));

            //console.log("Lista di lavorazioni:", lavorazioneList);
            setupAutocomplete(lavorazioneInput, lavorazioneAutocompleteList, lavorazioneList);
        } catch (error) {
            console.error("Errore nel caricamento dei dati lavorazione:", error);
        }
    }

    async function loadBarCodeData(jobId, mono, creationDate, operation) {
        console.log("Caricamento barcode per jobId:", jobId, ", mono:", mono, " e operation:", operation);
        if (!jobId || !mono || !creationDate || !operation) return;
        
        try {
            barcodeInput.disabled = false;
            const barCodeResult = await fetchJobsByLavorazione(jobId, mono, creationDate, operation);
            //console.log("Risultato barcode:", barCodeResult);
            barcodeList = barCodeResult
                .filter(barCode => barCode && barCode.barCode && barCode.itemDesc && barCode.component)
                .map(barCode => ({
                    component: barCode.component,
                    barCode: barCode.barCode,
                    itemDesc: barCode.itemDesc,
                    display: `Item: ${barCode.component} - Code: ${barCode.barCode} - ${barCode.itemDesc}`
                }));

            //console.log("Lista di barcode:", barcodeList);
            setupAutocomplete(barcodeInput, barcodeAutocompleteList, barcodeList);
        } catch (error) {
            console.error("Errore nel caricamento dei dati barcode:", error);
        }
    }

    async function loadAllData(jobId, mono, creationDate, operation, barCode) {
        if (!jobId || !mono || !creationDate || !operation || !barCode) return null;
        
        try {
            const allDataResult = await fetchJobsByBarCode(jobId, mono, creationDate, operation, barCode);
            console.log("Lista di tutti i dati:", allDataResult);

            if(allDataResult.length > 0) {

                var sum = 0;
                // Dati lista temporanea
                dataResultList.forEach(element => {
                    if(element.component === allDataResult[0].component) {
                        sum += parseFloat(element.prelQty || 0);
                    }
                });
                console.log("Somma delle quantità della lista temporanea:", sum);
                console.log("Component:", allDataResult[0].component);
                const prelMatQtyList = await fetchA3PrelMatQtyList(allDataResult[0].component);
                console.log("Lista di quantità da A3_app_prel_mat:", prelMatQtyList);

                if(prelMatQtyList.length > 0) {
                    prelMatQtyList.forEach(element => {
                        sum += parseFloat(element.prelQty || 0);
                    });
                }

                console.log("Somma totale:", sum);

                quantitaInput.disabled = false;
                // Il valore comincia dalla quantità disponibile meno quella già prelevata
                const finalSum =  parseFloat(allDataResult[0].prelResQty || 0) - sum;
                quantitaInput.value = finalSum;
                
                const quantitaLabel = document.querySelector('label[for="prel-mat-quantita"]');
                if (quantitaLabel) {
                    quantitaLabel.textContent = `Qta. da prelevare: ${allDataResult[0].prelResQty} - Qta. prelevabile: ${finalSum} - UoM: ${allDataResult[0].prelUoM}`;
                }

                // Se la quantità è negativa o maggiore di prelResQty, mostra un messaggio di errore
                if (parseFloat(quantitaInput.value) < 0) {
                    errorQty.style.display = "block";
                }

                allDataResult[0].finalSum = finalSum;

                return allDataResult[0];
            }
            else {
                console.error("Nessun dato trovato per i parametri forniti.");
                return null;
            }

        } catch (error) {
            console.error("Errore nel caricamento dei dati:", error);
            return null;
        }
    }
});

// Funzioni di fetch
async function fetchJobMostep(job) {
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/mostepsmocomponent/job`, {
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
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/mostepsmocomponent/mono`, {
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

async function fetchJobsByBarCode(job, mono, creationDate, operation, barCode) {
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/mostepsmocomponent/barcode`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                "job": job,
                "mono": mono,
                "creationDate": creationDate,
                "operation": operation,
                "barCode": barCode
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
    if(globalAllData) return globalAllData;
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/job`, {
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

async function fetchA3PrelMatQtyList(component) {
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/prel_mat/get_prel_mat_with_component`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"component": component}),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return [];
        }

        const qtyList = await request.json();
        return qtyList;
    } catch (error) {
        console.error("Errore durante la fetch:", error);
        return [];
    }
};

async function fetchJobsByLavorazione(job, mono, creationDate, operation) {
    try {
        const request = await fetchWithAuth(`http://${getIPString()}:5245/api/mostepsmocomponent/operation`, {
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
    const list = document.getElementById("prel-mat-lista-temp");
    const noContent = document.getElementById("nocontent");
    const newItem = document.createElement("li");
    newItem.classList.add("just-added"); // Aggiungi classe per l'animazione

    newItem.innerHTML = `
        <div class="item-content"><div><spam class="item-content-heading">Comm:</spam> ${data.job} - <spam class="item-content-heading">MoId:</spam> ${data.moid} - <spam class="item-content-heading">MoNo:</spam> ${data.mono}</div>
        <div><spam class="item-content-heading">Lav:</spam> ${data.operation} - <spam class="item-content-heading">Desc:</spam> ${data.operDesc} </div>
        <div><spam class="item-content-heading">Item:</spam> ${data.component} - <spam class="item-content-heading">Code:</spam> ${data.barCode} </div>
        <div class=temp-list-qta-${data.moid}><strong>Qta: ${data.prelQty}</strong></div></div>
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