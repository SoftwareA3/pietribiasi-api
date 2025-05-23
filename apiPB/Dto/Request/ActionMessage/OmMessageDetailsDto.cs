using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class OmMessageDetailsDto
    {
        public int? MessageId { get; set; }

        public string? MessageType { get; set; }

        public DateTime? MessageDate { get; set; }

        public string? MessageText { get; set; }
    }
}