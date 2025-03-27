using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using apiPB.Models;

namespace apiPB.Repository.Abstraction
{
    public interface IVwApiJobRepository
    {
        IEnumerable<VwApiJob> GetVwApiJobs();
    }
}