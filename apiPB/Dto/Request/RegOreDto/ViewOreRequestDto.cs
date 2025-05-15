using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiPB.Dto.Request
{
    public class ViewOreRequestDto : A3AppViewRequestBaseDto
    {
        public string? Job { get; set; }
        public string? Operation { get; set; }
        public string? Mono { get; set; }
    }
}