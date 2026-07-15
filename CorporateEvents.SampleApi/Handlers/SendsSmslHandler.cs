using CorporateEvents.Abstractions.Handlers;
using CorporateEvents.SampleApi.Events;

namespace CorporateEvents.SampleApi.Handlers
{
    public class SendsSmslHandler : IEventHandler<PolicyIssuedEvent>
    {
        public async Task HandleAsync(PolicyIssuedEvent EventPolicy)
        {
            Console.WriteLine($"Enviando mensaje a {EventPolicy.Phone}");

            await Task.Delay(500);
        }
    }
}
