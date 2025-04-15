using apiPB.Models;
using apiPB.Filters;

namespace apiPB.Repository.Abstraction
{
    public interface IRegOreRepository
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella A3_app_reg_ore
        /// </summary>
        /// <returns>
        /// IEnumerable di A3AppRegOre: restituisce una collezione generica di modelli A3AppRegOre
        /// </returns>
        IEnumerable<A3AppRegOre> GetAppRegOre();

        /// <summary>
        /// Inserisce la lista di A3AppRegOre nel database
        /// </summary>
        /// <param name="IEnumerable<a3AppRegOreList>">Lista di A3AppRegOre da inserire</param>
        /// <returns>
        /// IEnumerable di A3AppRegOre: restituisce una collezione generica di modelli A3AppRegOre
        IEnumerable<A3AppRegOre> PostRegOreList(IEnumerable<RegOreFilter> filterList);

        /// <summary>
        /// Restituisce le informazioni della tabella A3_app_reg_ore filtrate per in base ai parametri opzionali inseriti
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: FromDateTime, ToDateTime, Job, Operation, Mono</param>
        /// <returns>
        /// IEnumerable di A3AppRegOre: restituisce una collezione generica di modelli A3AppRegOre
        /// </returns>
        IEnumerable<A3AppRegOre> GetAppViewOre(ViewOreRequestFilter filter);

        /// <summary>
        /// Aggiorna la riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: RegOreId, WorkingTime</param>
        /// <returns>
        /// A3AppRegOre: restituisce l'elemento modificato
        /// </returns>
        A3AppRegOre PutAppViewOre(ViewOrePutFilter filter);

        /// <summary>
        /// Elimina la riga della tabella A3_app_reg_ore in base al filtro passato
        /// </summary>
        /// <param name="filter">Filtro per l'esecuzione della query. Richiede le proprietà: RegOreId</param>
        /// <returns>
        /// A3AppRegOre: restituisce l'elemento eliminato
        /// </returns>
        A3AppRegOre DeleteRegOreId(ViewOreDeleteRequestFilter filter);
    }
}