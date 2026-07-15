using CorporateEvents.Abstractions.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Contracts
{
    public interface IIdempotencyService
    {
        Task<bool> TryStartProcessingAsync(IdempotencyDto idempotencyDto);
        Task CompleteProcessingAsync(Guid eventId);
        Task FailProcessingAsync(Guid eventId, string errorMessage);
    }
}
