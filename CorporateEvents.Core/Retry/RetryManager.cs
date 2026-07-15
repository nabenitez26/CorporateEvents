using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Core.Retry
{
    public class RetryManager
    {
        public static async Task ExecuteAsync(Func<Task> action, int maxRetries, int delayMilliseconds, ILogger logger = null)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    await action();
                    break; 
                }
                catch (Exception ex)
                {
                    retryCount++;
                    if (retryCount > maxRetries)
                    {
                        logger?.LogError(ex, "Maxima cantidad de reintentos. Action fallida.");
                        throw;
                    }
                    logger?.LogWarning(ex, "Error al ejecutar la acción. Reintentando {RetryCount}/{MaxRetries}...", retryCount, maxRetries);
                    await Task.Delay(delayMilliseconds); 
                }
            }
        }
    }
}
