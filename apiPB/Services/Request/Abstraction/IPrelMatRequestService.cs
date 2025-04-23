using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IPrelMatRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella A3_app_prel_mat
        /// </summary>
        /// <returns>
        /// IEnumerable di PrelMatDto: restituisce una collezione generica di PrelMatDto
        /// </returns>
        IEnumerable<PrelMatDto> GetAppPrelMat();

        /// <summary>
        /// Inserisce la lista di A3AppPrelMat nel database
        /// </summary>
        /// <param name="requestList">Lista di A3AppPrelMat da inserire</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat: restituisce una collezione generica di modelli A3AppPrelMat
        /// </returns>
        IEnumerable<PrelMatDto> PostPrelMatList(IEnumerable<PrelMatRequestDto> requestList);

        /// <summary>
        /// Aggiorna la lista di A3AppPrelMat nel database
        /// </summary>
        /// <param name="request"> Dto di richiesta</param>
        /// <returns>
        /// IEnumerable di A3AppPrelMat:  restituisce una collezione generica di Dto A3AppPrelMatDto
        /// </returns>
        IEnumerable<PrelMatDto> GetViewPrelMatList(ViewPrelMatRequestDto request);

        /// <summary>
        /// Aggiorna la riga della tabella A3_app_prel_mat dato A3AppViewMatPutRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// A3AppPrelMatDto: restituisce l'elemento modificato
        /// </returns>
        PrelMatDto? PutViewPrelMat(ViewPrelMatPutRequestDto request);

        /// <summary>
        /// Elimina la riga della tabella A3_app_prel_mat dato A3AppDeleteRequestDto
        /// </summary>
        /// <param name="request">Dto di richiesta</param>
        /// <returns>
        /// A3AppPrelMatDto: restituisce l'elemento eliminato
        /// </returns>
        PrelMatDto DeletePrelMatId(ViewPrelMatDeleteRequestDto request);
    }
}