using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Abstractions.Events;
using CorporateEvents.Abstractions.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Core.Subscriptions
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly Dictionary<Type, List<Type>> _eventSubscriptions = new Dictionary<Type, List<Type>>();
        public void AddSubscription<TEvent, THandler>()
            where TEvent : EventBase
            where THandler : IEventHandler<TEvent>
        {
            var typeOfEvent = typeof(TEvent); 
            var typeOfHandler = typeof(THandler);
            if (!_eventSubscriptions.ContainsKey(typeOfEvent))
            {
                _eventSubscriptions[typeOfEvent] = new List<Type>();
            }
            if (!_eventSubscriptions[typeOfEvent].Contains(typeOfEvent))
            {
                _eventSubscriptions[typeOfEvent].Add(typeOfHandler);
            }
        }

        public IReadOnlyCollection<Type> GetHandlersForEvent<TEvent>() where TEvent : EventBase
        {
            var typeOfEvent = typeof(TEvent);
            if (_eventSubscriptions.TryGetValue(typeOfEvent, out var handlers))
            {
                return handlers.AsReadOnly();
            }
            return [];
        }

        public bool HasSubscriptionsForEvent<TEvent>() where TEvent : EventBase
        {
            return _eventSubscriptions.ContainsKey(typeof(TEvent));
        }
    }
}
