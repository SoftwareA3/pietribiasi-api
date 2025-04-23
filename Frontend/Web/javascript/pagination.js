export function createPagination(ulSelector, itemsPerPage = 6, maxPageButtons = 5, labels = {
    prev: 'Precedente',
    next: 'Successiva',
    ellipsis: '...'
}) {
    // Seleziona l'elemento UL
    const ulElement = document.querySelector(ulSelector);
    if (!ulElement) {
    console.error(`Elemento ${ulSelector} non trovato`);
    return;
    }
    
    // Ottieni tutti gli elementi LI
    const items = Array.from(ulElement.querySelectorAll('li'));
    const totalItems = items.length;
    console.log("Elementi totali: " + totalItems);
    const totalPages = Math.ceil(totalItems / itemsPerPage);
    console.log("Pagine totali: " + totalPages);
    
    // Se non ci sono abbastanza elementi per la paginazione, esci
    if (totalItems <= itemsPerPage) {
    return;
    }

    // Se c'è solo una pagina, non mostrare la paginazione
    if (totalPages === 1) {
        console.log("Pagine totali: " + totalPages);
        console.log("Nessuna paginazione da mostrare");
        return;
    }
    
    // Variabile per tenere traccia della pagina corrente
    let currentPage = 1;
    
    // Crea il container per i controlli di paginazione
    const paginationContainer = document.createElement('div');
    paginationContainer.className = 'pagination-controls';
    ulElement.parentNode.insertBefore(paginationContainer, ulElement.nextSibling);
    
    // Funzione per mostrare gli elementi della pagina corrente
    function showCurrentPage() {
    const startIndex = (currentPage - 1) * itemsPerPage;
    const endIndex = Math.min(startIndex + itemsPerPage, totalItems);
    
    // Nascondi tutti gli elementi
    items.forEach(item => item.style.display = 'none');
    
    // Mostra solo gli elementi della pagina corrente
    for (let i = startIndex; i < endIndex; i++) {
        items[i].style.display = '';
    }
    
    // Aggiorna i controlli di paginazione
    updatePaginationControls();
    }
    
    // Funzione per aggiornare i controlli di paginazione
    function updatePaginationControls() {
    // Aggiorna solo lo stato dei pulsanti esistenti
    const existingButtons = paginationContainer.querySelectorAll('.pagination-button, .pagination-ellipsis');
    if (existingButtons.length > 0) {
        paginationContainer.querySelectorAll('.pagination-button').forEach(button => {
            const pageNum = parseInt(button.textContent, 10);
            if (!isNaN(pageNum)) {
                button.classList.toggle('active', pageNum === currentPage);
            }
        });
        return; // Evita di ricreare i pulsanti
    }

    // Se non ci sono controlli, crea i pulsanti
    paginationContainer.innerHTML = '';

    // Pulsante pagina precedente
    const prevButton = createButton(labels.prev);
    prevButton.className += ' next-prev-button';
    prevButton.addEventListener('click', () => {
        if (currentPage > 1) {
            currentPage--;
            showCurrentPage();
        }
    });
    paginationContainer.appendChild(prevButton);

    // Calcola quali pulsanti pagina mostrare
    let startPage = Math.max(1, currentPage - Math.floor(maxPageButtons / 2));
    let endPage = Math.min(totalPages, startPage + maxPageButtons - 1);

    // Aggiusta se siamo vicini alla fine
    if (endPage - startPage + 1 < maxPageButtons) {
        startPage = Math.max(1, endPage - maxPageButtons + 1);
    }

    // Mostra ellipsis e prima pagina se necessario
    if (startPage > 1) {
        paginationContainer.appendChild(createPageButton(1));
        if (startPage > 2) {
            const ellipsis = document.createElement('span');
            ellipsis.textContent = labels.ellipsis;
            ellipsis.className = 'pagination-ellipsis';
            paginationContainer.appendChild(ellipsis);
        }
    }

    // Pulsanti pagina numerati
    for (let i = startPage; i <= endPage; i++) {
        paginationContainer.appendChild(createPageButton(i));
    }

    // Mostra ellipsis e ultima pagina se necessario
    if (endPage < totalPages) {
        if (endPage < totalPages - 1) {
            const ellipsis = document.createElement('span');
            ellipsis.textContent = labels.ellipsis;
            ellipsis.className = 'pagination-ellipsis';
            paginationContainer.appendChild(ellipsis);
        }
        paginationContainer.appendChild(createPageButton(totalPages));
    }

    // Pulsante pagina successiva
    const nextButton = createButton(labels.next);
    nextButton.className += ' next-prev-button';
    nextButton.addEventListener('click', () => {
        if (currentPage < totalPages) {
            currentPage++;
            showCurrentPage();
        }
    });
    paginationContainer.appendChild(nextButton);
    }
    
    // Funzione per creare un pulsante generico
    function createButton(text) {
    const button = document.createElement('button');
    button.textContent = text;
    button.className = 'pagination-button';
    return button;
    }
    
    // Funzione per creare un pulsante pagina numerato
    function createPageButton(pageNum) {
    const button = createButton(pageNum);
    if (pageNum === currentPage) {
        button.className += ' active';
    }
    button.addEventListener('click', () => {
        currentPage = pageNum;
        showCurrentPage();
    });
    return button;
    }
    
    // Inizializza la visualizzazione
    showCurrentPage();
    
    // Restituisci un oggetto con metodi pubblici
    return {
    goToPage: (pageNum) => {
        if (pageNum >= 1 && pageNum <= totalPages) {
        currentPage = pageNum;
        showCurrentPage();
        }
    },
    nextPage: () => {
        if (currentPage < totalPages) {
        currentPage++;
        showCurrentPage();
        }
    },
    prevPage: () => {
        if (currentPage > 1) {
        currentPage--;
        showCurrentPage();
        }
    },
    getCurrentPage: () => currentPage,
    getTotalPages: () => totalPages,
    refresh: () => showCurrentPage(),
    updatePaginationControls: () => updatePaginationControls()
    };
}