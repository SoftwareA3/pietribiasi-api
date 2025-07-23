using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero delle informazioni dalla vista vw_api_mosteps_mocomponent.
    /// I parametri passati sono parametri per il recupero dei dati.
    /// La classe deriva da OperationRequestDto per ereditare gli attributi comuni
    /// </summary>
    public class BarCodeRequestDto : OperationRequestDto
    {
        public string? BarCode { get; set; } = string.Empty;
        public string? Component { get; set; } = string.Empty;
    }
}