import { fetchWithAuth } from "./fetch.js";

/*
    Fetch di tutti i jobs e creazione lista di autocompletamento

    Cerca con il job selezionato come parametro. Crea lista di autocompletamento o lista di tutte
    le lavorazioni e gli ordini di produzione associati e crea lista di autocompletamento
*/

document.addEventListener("DOMContentLoaded", async function () {
    // recupera tutti gli input e le liste di autocompletamento 
    
    const commessaInput = document.getElementById("reg-ore-commessa");
    const commessaAutocompleteList = document.getElementById("reg-ore-commessa-autocomplete-list");

    const lavorazioneInput = document.getElementById("reg-ore-lavorazione");
    const lavorazioneAutocompleteList = document.getElementById("reg-ore-lavorazione-autocomplete-list");

    const odpInput = document.getElementById("reg-ore-odp");
    const odpAutocompleteList = document.getElementById("reg-ore-odp-autocomplete-list");

    // Recupera tutti i lavori
    const result = await fetchAllJobs();
    var jobList = [];

    result.forEach((job) => {
        if (job && job.job && job.description) {
            jobList.push({
                job: job.job,
                description: job.description,
                display: Object.values(job).join(" - ")
            });
            console.log("job in jobList mentre viene popolata: ", Object.values(job).join(" - "));
        }
    });

    console.log(typeof jobList);

    console.log("Lista di lavori:", jobList);

    // Variabili per memorizzare le selezioni
    let selectedCommessa = null;
    let selectedOdp = null;
    let selectedLavorazione = null;

    // Funzione ausiliaria per trovare la commessa corrispondente al testo inserito
    function findCommessaByDisplay(displayText) {
        return jobList.find(item => item.display === displayText);
    }

    // Funzione ausiliaria per trovare l'ODP corrispondente al testo inserito
    function findOdpByDisplay(displayText, odpList) {
        return odpList.find(item => item.display === displayText);
    }

    // Setup autocomplete per la commessa
    setupAutocomplete(commessaInput, commessaAutocompleteList, jobList, function(selected) {
        selectedCommessa = selected;
        // Puoi eseguire altre azioni qui, come pulire gli altri campi
        odpInput.value = "";
        lavorazioneInput.value = "";
        selectedOdp = null;
        selectedLavorazione = null;
    });

    // Setup autocomplete per l'ordine di produzione
    odpInput.addEventListener("focus", async function() {
        // Verifica se il campo commessa ha un valore
        if (commessaInput.value && !selectedCommessa) {
            // Cerca di trovare la commessa corrispondente
            selectedCommessa = findCommessaByDisplay(commessaInput.value);
        }
        
        if (!selectedCommessa) {
            alert("Seleziona prima una commessa");
            commessaInput.focus();
            return;
        }

        try {
            const moStep = await fetchJobMostep(selectedCommessa.job);
            const odpList = [];

            moStep.forEach((item) => {
                if (item && item.mono && item.creationDate) {
                    odpList.push({
                        mono: item.mono,
                        creationDate: item.creationDate,
                        display: `${item.mono} - ${item.creationDate}`
                    });
                }
            });

            console.log("Lista di ODP:", odpList);

            // Verifica se il campo ODP ha già un valore
            if (odpInput.value && !selectedOdp) {
                selectedOdp = findOdpByDisplay(odpInput.value, odpList);
            }

            // Forza l'aggiornamento della lista
            setupAutocomplete(odpInput, odpAutocompleteList, odpList, function(selected) {
                selectedOdp = selected;
                lavorazioneInput.value = "";
                selectedLavorazione = null;
            });

            // Mostra la lista immediatamente
            showAutocompleteList(odpInput, odpAutocompleteList, odpList);
        } catch (error) {
            console.error("Errore:", error);
        }
    });

    // Setup autocomplete per la lavorazione
    lavorazioneInput.addEventListener("focus", async function() {
        // Verifica se il campo commessa ha un valore
        if (commessaInput.value && !selectedCommessa) {
            // Cerca di trovare la commessa corrispondente
            selectedCommessa = findCommessaByDisplay(commessaInput.value);
        }
        
        // Verifica se c'è già un valore nel campo ODP ma non è stato selezionato
        if (odpInput.value && !selectedOdp && selectedCommessa) {
            // Recupera la lista ODP e cerca di trovare l'ODP corrispondente
            const moStep = await fetchJobMostep(selectedCommessa.job);
            const odpList = [];
            
            moStep.forEach((item) => {
                if (item && item.mono && item.creationDate) {
                    odpList.push({
                        mono: item.mono,
                        creationDate: item.creationDate,
                        display: `${item.mono} - ${item.creationDate}`
                    });
                }
            });
            
            selectedOdp = findOdpByDisplay(odpInput.value, odpList);
        }
        
        if (!selectedCommessa || !selectedOdp) {
            if (!selectedCommessa) {
                alert("Seleziona prima una commessa");
                commessaInput.focus();
            } else {
                alert("Seleziona prima un ordine di produzione");
                odpInput.focus();
            }
            return;
        }
        
        try {
            const mostepOdp = await fetchJobMostepWithOdp(
                selectedCommessa.job, 
                selectedOdp.mono, 
                selectedOdp.creationDate
            );
            console.log("Mostep ODP:", mostepOdp);
            // Crea la lista di lavorazioni...
            const lavorazioniList = []; // costruisci la lista qui
            mostepOdp.forEach((item) => {
                if (item && item.operation && item.operDesc) {
                    lavorazioniList.push({
                        operation: item.operation,
                        operDesc: item.operDesc,
                        display: `${item.operation} - ${item.operDesc}`
                    });
                    console.log("Lavorazione in lista:", item.operDesc);
                }
            });
            console.log("Lista di lavorazioni:", lavorazioniList);
            
            setupAutocomplete(lavorazioneInput, lavorazioneAutocompleteList, lavorazioniList, function(selected) {
                selectedLavorazione = selected;
            });

            // Mostra la lista immediatamente
            showAutocompleteList(lavorazioneInput, lavorazioneAutocompleteList, lavorazioniList);
        } catch (error) {
            console.error("Errore:", error);
        }
    });

    // Aggiungi listener diretto per il click sul pulsante "Cerca"
    const cercaButton = document.getElementById("reg-ore-cerca");
    cercaButton.addEventListener("click", function() {
        if (commessaInput.value) {
            selectedCommessa = findCommessaByDisplay(commessaInput.value);
            if (!selectedCommessa) {
                alert("Commessa non trovata");
                return;
            }
            
            // Pulisci i campi successivi
            odpInput.value = "";
            lavorazioneInput.value = "";
            selectedOdp = null;
            selectedLavorazione = null;
            
            // Sposta il focus sul campo ODP
            odpInput.focus();
        }
    });

    setupDeleteButtons();
});

// Funzione per mostrare immediatamente la lista di autocompletamento
function showAutocompleteList(inputElement, listElement, itemList) {
    // Svuota la lista e rendila visibile
    listElement.innerHTML = "";
    listElement.classList.remove("hidden");
    
    // Aggiungi tutti gli elementi alla lista
    itemList.forEach((item) => {
        if (item && item.display) {
            const itemDiv = document.createElement("div");
            itemDiv.textContent = item.display;
            
            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                // Imposta il valore dell'input come il display dell'elemento
                inputElement.value = item.display;
                // Chiudi la lista
                listElement.classList.add("hidden");
            });
            
            listElement.appendChild(itemDiv);
        }
    });
    
    // Aggiungi la classe scrollable se ci sono più di 3 elementi
    if (listElement.children.length > 3) {
        listElement.classList.add("scrollable");
    }
}

async function fetchJobMostep(job)
{
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/job/mostep", {
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

async function fetchJobMostepWithOdp(job, mono, creationDate)
{
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/job/mostep/odp", {
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

function setupAutocomplete(inputElement, listElement, fieldList, onSelectCallback) {
    let currentFocus = -1;

    // Quando l'input riceve il focus, mostra tutti gli elementi
    inputElement.addEventListener("focus", function() {
        // Mostra immediatamente tutti gli elementi disponibili
        showAutocompleteList(inputElement, listElement, fieldList);
    });
    
    // Event listener per l'input
    inputElement.addEventListener("input", function() {
        const value = this.value;
        
        // Chiudi la lista se è già aperta
        listElement.classList.add("hidden");
        
        // Se l'input è vuoto, mostra tutti gli elementi
        if (!value) {
            showAutocompleteList(inputElement, listElement, fieldList);
            return;
        }
        
        // Filtra gli elementi che corrispondono all'input
        const valueStr = value.toString().toUpperCase();
        const matchingItems = fieldList.filter(item => {
            if (!item || !item.display) return false;
            return item.display.toString().toUpperCase().includes(valueStr);
        });
        
        // Se non ci sono elementi corrispondenti, non mostrare la lista
        if (matchingItems.length === 0) {
            return;
        }
        
        // Resetta l'indice del focus
        currentFocus = -1;
        
        // Svuota la lista e rendila visibile
        listElement.innerHTML = "";
        listElement.classList.remove("hidden");
        
        // Aggiungi gli elementi corrispondenti alla lista
        matchingItems.forEach((item) => {
            const itemDiv = document.createElement("div");
            const itemStr = item.display;
            
            // Evidenzia la parte corrispondente
            const index = itemStr.toUpperCase().indexOf(valueStr);
            if (index !== -1) {
                itemDiv.innerHTML = itemStr.substring(0, index);
                itemDiv.innerHTML += "<strong>" + itemStr.substring(index, index + valueStr.length) + "</strong>";
                itemDiv.innerHTML += itemStr.substring(index + valueStr.length);
            } else {
                itemDiv.innerHTML = itemStr;
            }
            
            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                // Imposta il valore dell'input come il display dell'elemento
                inputElement.value = item.display;
                // Esegui il callback passando l'oggetto completo
                if (typeof onSelectCallback === 'function') {
                    onSelectCallback(item);
                }
                console.log("Elemento selezionato:", item);
                // Chiudi la lista
                listElement.classList.add("hidden");
            });
            
            listElement.appendChild(itemDiv);
        });
        
        // Aggiungi la classe scrollable se ci sono più di 3 elementi
        if (listElement.children.length > 3) {
            listElement.classList.add("scrollable");
        }
    });
    
    // Event listener per la navigazione con tastiera
    inputElement.addEventListener("keydown", function(e) {
        const items = listElement.getElementsByTagName("div");
        
        // Freccia giù
        if (e.key === "ArrowDown") {
            currentFocus++;
            addActive(items);
            e.preventDefault(); // Previene lo scorrimento della pagina
        } 
        // Freccia su
        else if (e.key === "ArrowUp") {
            currentFocus--;
            addActive(items);
            e.preventDefault(); // Previene lo scorrimento della pagina
        } 
        // Invio
        else if (e.key === "Enter") {
            e.preventDefault(); // Previene l'invio del form
            if (currentFocus > -1 && items.length > 0) {
                items[currentFocus].click();
            }
        } 
        // Esc
        else if (e.key === "Escape") {
            listElement.classList.add("hidden");
        }
    });
    
    // Focus out: chiudi la lista quando l'utente clicca altrove
    document.addEventListener("click", function(e) {
        if (e.target !== inputElement) {
            listElement.classList.add("hidden");
        }
    });
    
    // Funzione per evidenziare l'elemento attivo
    function addActive(items) {
        if (!items || items.length === 0) return;
        
        // Rimuovi prima la classe active da tutti gli elementi
        removeActive(items);
        
        // Correggi l'indice se necessario
        if (currentFocus >= items.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = items.length - 1;
        
        // Aggiungi la classe active all'elemento corrente
        items[currentFocus].classList.add("autocomplete-active");
        
        // Assicurati che l'elemento sia visibile (scroll)
        items[currentFocus].scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
    
    // Rimuovi la classe active da tutti gli elementi
    function removeActive(items) {
        for (let i = 0; i < items.length; i++) {
            items[i].classList.remove("autocomplete-active");
        }
    }
}

function setupDeleteButtons() {
    // Configura i pulsanti di eliminazione esistenti nella lista temporanea
    const deleteButtons = document.querySelectorAll("#reg-ore-lista-temp .delete");
    deleteButtons.forEach(button => {
        button.addEventListener("click", function() {
            const listItem = this.closest("li");
            if (listItem) {
                listItem.remove();
            }
        });
    });
}