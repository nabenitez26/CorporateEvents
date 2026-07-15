using CorporateEvents.Abstractions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Contracts
{
    public interface IMetricsService
    {
        Task<MetricsDto> GetMetricsAsync();
    }
}
