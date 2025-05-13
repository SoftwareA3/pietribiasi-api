using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Models
{
    public class SettingsDto
    {
        public string? MagoUrl { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public int? SpecificatorType { get; set; }

        public bool? Closed { get; set; }
    }
}