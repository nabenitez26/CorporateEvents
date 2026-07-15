using CorporateEvents.Abstractions.Handlers;
using CorporateEvents.SampleApi.Events;

namespace CorporateEvents.SampleApi.Handlers
{
    public class SendEmailHandler : IEventHandler<PolicyIssuedEvent>
    {
        public async Task HandleAsync(PolicyIssuedEvent EventPolicy)
        {
            /*Console.WriteLine($"Enviando correo a {EventPolicy.Email}");

            await Task.Delay(500);*/
            throw new Exception("Error de prueba");
        }
    }
}
