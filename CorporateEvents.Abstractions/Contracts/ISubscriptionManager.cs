using CorporateEvents.Abstractions.Events;
using CorporateEvents.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Abstractions.Contracts
{
    public interface ISubscriptionManager
    {
        void AddSubscription<TEvent, THandler>()
            where TEvent : EventBase
            where THandler : IEventHandler<TEvent>;

        IReadOnlyCollection<Type> GetHandlersForEvent<TEvent>() where TEvent : EventBase;

        bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase;
    }
}
