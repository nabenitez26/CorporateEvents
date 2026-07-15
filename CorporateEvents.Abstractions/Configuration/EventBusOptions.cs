using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Configuration
{
    public class EventBusOptions
    {
        public const string SectionName = "EventBus";
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 1000;
        public string ConnectionString{ get; set; }
    }
}
