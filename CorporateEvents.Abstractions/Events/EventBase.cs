using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Events
{
    public abstract class EventBase
    {
        public Guid EventId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime OccurredOnUTC { get; set; } = DateTime.UtcNow;
    }
}
