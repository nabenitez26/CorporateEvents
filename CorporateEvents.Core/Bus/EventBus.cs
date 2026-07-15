using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Abstractions.Events;
using CorporateEvents.Abstractions.Handlers;
using CorporateEvents.Abstractions.Configuration;
using CorporateEvents.Core.Retry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CorporateEvents.Abstractions.Dtos;
using System.Text.Json;
using System.Collections.Concurrent;

namespace CorporateEvents.Core.Bus
{
    public class EventBus : IEventBus
    {
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventBus> _logger;
        private readonly IDeadLetterQueueService _deadLetterQueueService;
        private readonly IIdempotencyService _idempotencialService;
        private readonly int _maxRetryAttempts; // Número máximo de intentos de reintento
        private readonly int _retryDelayMilliseconds; // Retraso entre intentos en milisegundos

        public EventBus(ISubscriptionManager subscriptionManager, IServiceProvider serviceProvider, IDeadLetterQueueService deadLetterQueueService, IIdempotencyService idempotencialService, ILogger<EventBus> logger, IOptions<EventBusOptions> options)
        {
            _subscriptionManager = subscriptionManager;
            _serviceProvider = serviceProvider;
            _deadLetterQueueService = deadLetterQueueService;
            _idempotencialService = idempotencialService;
            _logger = logger;
            _maxRetryAttempts = options.Value.RetryCount;
            _retryDelayMilliseconds = options.Value.RetryDelayMilliseconds;
        }

        public async Task Publish<TEvent>(TEvent eventData) where TEvent : EventBase
        {
            _logger.LogInformation("Publicacion de evento: {EventType}. EventId: {EventId}. CorrelationId: {CorrelationId}",
                    typeof(TEvent).Name, eventData.EventId, eventData.CorrelationId);
            if (!_subscriptionManager.HasSubscriptionsForEvent<TEvent>())
            {
                _logger.LogWarning("No hay suscripciones para el evento: {EventType}. EventId: {EventId}. CorrelationId: {CorrelationId}",
                    typeof(TEvent).Name, eventData.EventId, eventData.CorrelationId);
                return;
            }
            var handlers = _subscriptionManager.GetHandlersForEvent<TEvent>();
            if (!await _idempotencialService.TryStartProcessingAsync(new IdempotencyDto
            {
                EventId = eventData.EventId,
                CorrelationId = eventData.CorrelationId,
                EventType = typeof(TEvent).Name
            })) 
            {
                return;
            }
            ConcurrentBag<Exception> errors = new();
            var handlerTasks = handlers.Select(async handlerType =>
            {
                var handler = _serviceProvider.GetRequiredService(handlerType);
                try
                {
                    _logger.LogInformation("Ejecutando handler {HandlerName} para Evento {EventType}. EventId: {EventId}. CorrelationId: {CorrelationId}",
                                        handlerType.Name,typeof(TEvent).Name,eventData.EventId,eventData.CorrelationId);
                    await RetryManager.ExecuteAsync(
                        () => ((dynamic)handler).HandleAsync(eventData),
                        _maxRetryAttempts,
                        _retryDelayMilliseconds,
                        _logger
                    );
                    _logger.LogInformation("Handler {HandlerName} ejecutado exitosamente para Evento {EventType}. EventId: {EventId}. CorrelationId: {CorrelationId}",
                                        handlerType.Name, typeof(TEvent).Name, eventData.EventId, eventData.CorrelationId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en la ejecucion del Handler {HandlerType} para Evento {EventType}. EventId: {EventId}. CorrelationId: {CorrelationId} con el maximo número {MaxRetryAttempts} de intentos",
                                        handlerType.Name, typeof(TEvent).Name, eventData.EventId, eventData.CorrelationId, _maxRetryAttempts);
                        errors.Add(ex);
                        try
                        {
                            await _deadLetterQueueService.SaveAsync(new DeadLetterDto
                            {
                                EventId = eventData.EventId,
                                CorrelationId = eventData.CorrelationId,
                                EventType = typeof(TEvent).Name,
                                HandlerName = handlerType.Name,
                                PayloadJson = JsonSerializer.Serialize(eventData, typeof(TEvent)),
                                ErrorMessage = ex.ToString(),
                                FailedAt = DateTimeOffset.UtcNow
                            });
                        }
                        catch (Exception dlqEx)
                        {
                            // Última línea de defensa: si ni la DLQ funciona, al menos queda en el log
                            _logger.LogCritical(dlqEx,"No se pudo persistir en la DLQ el evento {EventId} del handler {HandlerType}. El evento se perdió, correlationId: {correlationId}",
                                eventData.EventId, handlerType.Name, eventData.CorrelationId);
                        }
                       
                }
            }).ToList();
            await Task.WhenAll(handlerTasks);
            if (errors.Any())
            {
                await _idempotencialService.FailProcessingAsync(eventData.EventId, "Error en la ejecucion de uno o mas Handlers");
            }
            else
            {
                await _idempotencialService.CompleteProcessingAsync(eventData.EventId);
            }
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionManager.AddSubscription<TEvent, TEventHandler>();
        }
    }
}
