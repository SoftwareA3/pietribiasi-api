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

    const odlInput = document.getElementById("reg-ore-odp");
    const odlAutocompleteList = document.getElementById("reg-ore-odp-autocomplete-list");

    // Recupera tutti i lavori
    const result = await fetchAllJobs();
    var jobList = [];
    var descriptionList = [];

    result.forEach((job) => {
        if (job && job.job && job.description) {
            jobList.push(job.job);
            descriptionList.push(job.description);
        }
    });

    console.log(typeof jobList);

    console.log("Lista di lavori:", jobList);
    console.log("Lista di descrizioni:", descriptionList);

    // Recupero preemptive delle informazioni di tutti i lavori disponibili
    
    var operationsList = [];
    var altRtgStepList = [];
    var jobInfoList = [];

    jobInfoList = await Promise.all(jobList.map(job => fetchJobMostep(job)));
    console.log("Lista di lavori con informazioni:", jobInfoList);

    jobInfoList.forEach((job, i) => {
        console.log("Job in jobInfoList:", job[i]);
        if (job[i] !== null && job[i].operation !== null && job[i].altRtgStep !== null) {
            operationsList.push(job[i].operation);
            altRtgStepList.push(job[i].altRtgStep);
            console.log("Job operations:", job[i].operation);
            console.log("Job altRtgStep:", job[i].altRtgStep);
        }
        else {
            console.error("Job non valido o senza operazioni:", job[i]);
        }
    });



    /* Commessa nuova: */

    setupAutocomplete(lavorazioneInput, lavorazioneAutocompleteList, operationsList);
    setupAutocomplete(odlInput, odlAutocompleteList, altRtgStepList);

    // Selezionando un lavoro dalla lista di autocompletamento, si recuperano le informazioni
    // e si compilano i campi sottostanti

    // Stessa cosa per il campo "lavorazione"
    // Configura l'autocompletamento per il campo "commessa"
    setupAutocomplete(commessaInput, commessaAutocompleteList, jobList, descriptionList);

    /* Commessa esistente: */ 

    // Se la commessa esiste, premendo il tasto cerca si recuperano tutte le informazioni inerenti a 
    // quella commessa e i campi sotto si possono compilare automaticamente a partire da quelle informazion
    // Recupera tutti i dati con il tasto "Cerca", inviando il valore del campo "commessa"

    // Caricamento preemptive delle informazioni per ogni job

    // Salva una lista dei vari dati, iterando sul JSON. In pratica salva una lista di "job" e "lavorazione" e "odp"

    // Configura l'autocompletamento per il campo "lavorazione"

    // Configura l'autocompletamento per il campo "odp"

    // Imposta la gestione dei pulsanti di eliminazione esistenti
    setupDeleteButtons();
});

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
            return;
        }

        const jobInfo = await request.json();
        return jobInfo;
        
    } catch (error) {
        console.error("Errore durante la fetch:", error);
    }
}

async function fetchJobMostepComponent(job)
{
    try {
        const request = await fetchWithAuth("http://localhost:5245/api/job/mostepcomponent", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({"job": job}),
        });

        if (!request || !request.ok) {
            console.error("Errore nella richiesta:", request.status, request.statusText);
            return;
        }
        const jobInfo = await request.json();
        return jobInfo;
    }
    catch (error) {
        console.error("Errore durante la fetch:", error);
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
            return;
        }

        const jobs = await request.json();
        return jobs;
        
    } catch (error) {
        console.error("Errore durante la fetch:", error);
    }
}

function setupAutocomplete(inputElement, listElement, list, descriptionList = []) {
    let currentFocus = -1;

    inputElement.addEventListener("focus", function() {
        const itemDiv = document.createElement("div");
        listElement.innerHTML = "";
        listElement.classList.remove("hidden");
        list.forEach((item, i) => {
            const itemStr = item.toString();
            itemDiv.innerHTML += itemStr;

            if (descriptionList.length > i && descriptionList[i]) {
                itemDiv.innerHTML += " - " + descriptionList[i];
            }

            // Aggiungi un attributo data per il valore
            itemDiv.setAttribute("list-value", itemStr);
            
            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                inputElement.value = this.getAttribute("list-value");
                closeAutocompleteList();
            });

            listElement.appendChild(itemDiv);

            if(listElement.children.length > 3) {
                listElement.classList.add("scrollable");
            }
        });
    });
    
    // Event listener per l'input
    inputElement.addEventListener("input", function() {
        const value = this.value;
        
        // Chiudi la lista se è già aperta
        closeAutocompleteList();
        
        // Filtra gli elementi che corrispondono all'input
        // Converte sempre sia il valore di input che quello dell'elemento in stringhe per il confronto
        const valueStr = value.toString().toUpperCase();

        const matchingItems = list.filter(item => {
            if (item === null || item === undefined) return false;
            return item.toString().toUpperCase().includes(valueStr);
        });
        
        console.log("Valore di input:", value);
        console.log("Elementi corrispondenti:", matchingItems.length);
        
        // Resetta l'indice del focus
        currentFocus = -1;
        
        // Svuota la lista e rendila visibile
        listElement.innerHTML = "";
        listElement.classList.remove("hidden");
        
        // Aggiungi gli elementi alla lista
        matchingItems.forEach((item, i) => {
            const itemDiv = document.createElement("div");
            const itemStr = item.toString();
            
            // Evidenzia la parte corrispondente
            const index = itemStr.toUpperCase().indexOf(valueStr);
            if (index !== -1) {
                itemDiv.innerHTML = itemStr.substring(0, index);
                itemDiv.innerHTML += "<strong>" + itemStr.substring(index, index + valueStr.length) + "</strong>";
                itemDiv.innerHTML += itemStr.substring(index + valueStr.length);
            } else {
                itemDiv.innerHTML = itemStr;
            }

            // Aggiungi descrizione se disponibile
            if (descriptionList.length > i && descriptionList[i]) {
                itemDiv.innerHTML += " - " + descriptionList[i];
            }
            
            // Aggiungi un attributo data per il valore
            itemDiv.setAttribute("list-value", itemStr);
            
            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                inputElement.value = this.getAttribute("list-value");
                closeAutocompleteList();
            });
            
            listElement.appendChild(itemDiv);
            if(listElement.children.length > 3) {
                listElement.classList.add("scrollable");
            }
        });
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
            closeAutocompleteList();
        }
    });
    
    // Focus out: chiudi la lista quando l'utente clicca altrove
    document.addEventListener("click", function(e) {
        if (e.target !== inputElement) {
            closeAutocompleteList();
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
    
    // Chiudi la lista di autocompletamento
    function closeAutocompleteList() {
        listElement.classList.add("hidden");
    }
}

// function addToTemporaryList(data) {
//     const list = document.getElementById("reg-ore-lista-temp");
//     const newItem = document.createElement("li");
//     newItem.classList.add("just-added"); // Aggiungi classe per l'animazione
    
//     newItem.innerHTML = `
//         <div class="item-content">${data}</div>
//         <div class="item-actions">
//             <button class="button-icon delete option-button" title="Rimuovi">
//                 <i class="fa-solid fa-xmark button-icon"></i>
//             </button>
//         </div>
//     `;
    
//     list.appendChild(newItem);
    
//     // Aggiungi event listener per il pulsante di eliminazione
//     const deleteButton = newItem.querySelector(".delete");
//     deleteButton.addEventListener("click", function() {
//         list.removeChild(newItem);
//     });
// }

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