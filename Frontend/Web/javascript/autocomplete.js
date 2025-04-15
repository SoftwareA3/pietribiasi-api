// Funzione per l'autocompletamento
// InputElement: input dove l'utente scrive
// ListElement: div dove mostrare i risultati
// List: array di oggetti da cui filtrare i risultati. list dev'essere un array di oggetti. 
// Ogni oggetto deve avere una proprietà "display" che contiene la stringa da mostrare nell'autocompletamento.
export function setupAutocomplete(inputElement, listElement, list) {
    let currentFocus = -1;

    // Gestisci il focus sull'input
    inputElement.addEventListener("focus", function() {
        showAllItems();
    });

    // Event listener per l'input
    inputElement.addEventListener("input", function() {
        const value = this.value.toString().toUpperCase();
        
        // Chiudi la lista esistente
        listElement.innerHTML = "";
        
        if (!value) {
            showAllItems();
            return;
        }
        
        // Filtra gli elementi che corrispondono all'input
        const matchingItems = list.filter(item => 
            item && item.display && item.display.toString().toUpperCase().includes(value)
        );

        // Resetta l'indice del focus
        currentFocus = -1;
        listElement.classList.remove("hidden");

        // Aggiungi gli elementi alla lista
        matchingItems.forEach((item) => {
            const itemDiv = document.createElement("div");
            const itemStr = item.display.toString();

            // Evidenzia la parte corrispondente
            const index = itemStr.toUpperCase().indexOf(value);
            if (index !== -1) {
                itemDiv.innerHTML = itemStr.substring(0, index);
                itemDiv.innerHTML += "<strong>" + itemStr.substring(index, index + value.length) + "</strong>";
                itemDiv.innerHTML += itemStr.substring(index + value.length);
            } else {
                itemDiv.textContent = itemStr;
            }

            // Event listener per il click sull'elemento
            itemDiv.addEventListener("click", function() {
                inputElement.value = itemStr;
                closeAutocompleteList();
                
                // Attiva manualmente l'evento change
                const event = new Event('change', { bubbles: true });
                inputElement.dispatchEvent(event);
            });

            listElement.appendChild(itemDiv);
        });
        
        if (listElement.children.length > 3) {
            listElement.classList.add("scrollable");
        }
    });

    // Funzione per mostrare tutti gli elementi
    function showAllItems() {
        listElement.innerHTML = "";
        listElement.classList.remove("hidden");
        
        list.forEach((item) => {
            const itemDiv = document.createElement("div");
            itemDiv.textContent = item.display;
            
            itemDiv.addEventListener("click", function() {
                inputElement.value = item.display;
                closeAutocompleteList();
                
                // Attiva manualmente l'evento change
                const event = new Event('change', { bubbles: true });
                inputElement.dispatchEvent(event);
            });
            
            listElement.appendChild(itemDiv);
        });
        
        if (listElement.children.length > 3) {
            listElement.classList.add("scrollable");
        }
    }

    // Event listener per la navigazione con tastiera
    inputElement.addEventListener("keydown", function(e) {
        const items = listElement.getElementsByTagName("div");

        // Freccia giù
        if (e.key === "ArrowDown") {
            currentFocus++;
            addActive(items);
            e.preventDefault();
        } 
        // Freccia su
        else if (e.key === "ArrowUp") {
            currentFocus--;
            addActive(items);
            e.preventDefault();
        } 
        // Invio
        else if (e.key === "Enter") {
            e.preventDefault();
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
