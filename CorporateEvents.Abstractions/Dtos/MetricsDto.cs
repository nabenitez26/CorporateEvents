using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Dtos
{
    public class MetricsDto
    {
        public int PublishedEvents { get; init; }
        public int ProcessedEvents { get; init; }
        public int FailedEvents { get; init; }
    }
}
