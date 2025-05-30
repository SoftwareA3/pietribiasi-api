using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    /// <summary>
    /// DTO per il recupero dei record filtrati per Component dalla tabella A3_app_prel_mat.
    /// I record sono utilizzati per il recupero delle quantità già salvate per il componente specificato.
    /// </summary>
    public class ComponentRequestDto
    {
        public string? Component { get; set; }
    }
}