using CorporateEvents.Abstractions.Configuration;
using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Abstractions.Dtos;
using CorporateEvents.Persistence.SqlServer.Idempotency;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Persistence.SqlServer.DeadLetter
{
    public class DeadLetterQueueService : IDeadLetterQueueService
    {
        private readonly string _connectionString;
        public DeadLetterQueueService(IOptions<EventBusOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }
        public async Task SaveAsync(DeadLetterDto record)
        {
            const string sql = @"
            INSERT INTO EventBus.DeadLetterEvents
                (EventId, CorrelationId, EventType, HandlerName, PayloadJson, ErrorMessage, FailedAt)
            VALUES
                (@EventId, @CorrelationId, @EventType, @HandlerName, @PayloadJson, @ErrorMessage, @FailedAt)";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EventId", record.EventId);
            command.Parameters.AddWithValue("@CorrelationId", record.CorrelationId);
            command.Parameters.AddWithValue("@EventType", record.EventType);
            command.Parameters.AddWithValue("@HandlerName", record.HandlerName);
            command.Parameters.AddWithValue("@PayloadJson", record.PayloadJson);
            command.Parameters.AddWithValue("@ErrorMessage", record.ErrorMessage);
            command.Parameters.AddWithValue("@FailedAt", record.FailedAt);
            await command.ExecuteNonQueryAsync();
        }
    }
}
