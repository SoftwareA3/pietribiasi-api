using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Dto.Models;
using apiPB.Dto.Request;

namespace apiPB.Services.Request.Abstraction
{
    public interface IInventarioRequestService
    {
        /// <summary>
        /// Restituisce tutte le informazioni dalla tabella vw_api_inventario
        /// </summary>
        /// <returns>
        /// IEnumerable di InventarioDto: restituisce una collezione generica di InventarioDto
        /// </returns>
        IEnumerable<InventarioDto> GetInventario();

        /// <summary>
        /// Inserisce la lista di InventarioDto nel database
        /// </summary>
        /// <param name="inventarioList">Lista di InventarioDto da inserire</param>
        /// <returns>
        /// IEnumerable di InventarioDto: restituisce una collezione generica di InventarioDto
        /// </returns>
        IEnumerable<InventarioDto> PostInventarioList(IEnumerable<InventarioRequestDto> inventarioList);
    }
}