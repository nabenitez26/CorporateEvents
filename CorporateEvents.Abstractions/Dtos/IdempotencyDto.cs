using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Dtos
{
    public class IdempotencyDto
    {
        public Guid EventId { get; init; }
        public Guid CorrelationId { get; init; }
        public string EventType { get; init; } = string.Empty;
        public DateTimeOffset ProcessedAt { get; init; }
        public string ErrorMessage { get; init; } = string.Empty;
    }
}
