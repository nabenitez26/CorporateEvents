using CorporateEvents.Abstractions.Events;
using CorporateEvents.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Contracts
{
    public interface IEventBus
    {
        Task Publish<TEvent>(TEvent eventData) where TEvent : EventBase;
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>;
    }
}
