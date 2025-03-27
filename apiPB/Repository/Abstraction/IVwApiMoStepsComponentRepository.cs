using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiMoStepsComponentRepository
    {
        IEnumerable<VwApiMoStepsComponent> GetVwApiMoStepsComponent(string job, short rtgStep, string alternate, short altRtgStep, short? position, string? component);
    }
}