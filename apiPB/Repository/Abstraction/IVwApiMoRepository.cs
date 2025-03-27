using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMoRepository
    {
        IEnumerable<VwApiMo> GetVwApiMo(string job, short rtgStep, string alternate, short altRtgStep);        
    }
}