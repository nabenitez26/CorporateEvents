using CorporateEvents.Abstractions.Handlers;
using CorporateEvents.SampleApi.Events;

namespace CorporateEvents.SampleApi.Handlers
{
    public class AuditHandler : IEventHandler<PolicyIssuedEvent>
    {
        public async Task HandleAsync(PolicyIssuedEvent EventPolicy)
        {
            Console.WriteLine($"Registrando auditoría para la póliza {EventPolicy.PolicyNumber}");

            await Task.Delay(300);
        }
    }
}
