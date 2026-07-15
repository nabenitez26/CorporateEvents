using CorporateEvents.Abstractions.Configuration;
using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Abstractions.Dtos;
using CorporateEvents.Abstractions.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Persistence.SqlServer.Idempotency
{
    public class SqlIdempotencyService : IIdempotencyService
    {
        private readonly string _connectionString;
        private readonly ILogger<SqlIdempotencyService> _logger;

        public SqlIdempotencyService(IOptions<EventBusOptions> options, ILogger<SqlIdempotencyService> logger)
        {
            _connectionString = options.Value.ConnectionString;
            _logger = logger;
        }

        public async Task CompleteProcessingAsync(Guid eventId)
        {
            const string sql = @"
            UPDATE EventBus.EventProcessing
                SET Status = @Status,
                    ProcessedAt = @ProcessedAt
            WHERE EventId = @EventId";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EventId", eventId);
            command.Parameters.AddWithValue("@ProcessedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Status", (int)ProcessingStatus.Completed);
            await command.ExecuteNonQueryAsync();
        }

        public async Task FailProcessingAsync(Guid eventId, string errorMessage)
        {
            const string sql = @"
            UPDATE EventBus.EventProcessing
                SET Status = @Status,
                    ErrorMessage = @ErrorMessage,
                    ProcessedAt = @ProcessedAt
            WHERE EventId = @EventId";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EventId", eventId);
            command.Parameters.AddWithValue("@ErrorMessage", errorMessage);
            command.Parameters.AddWithValue("@ProcessedAt", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Status", (int)ProcessingStatus.Failed);
            await command.ExecuteNonQueryAsync();
            
        }

        public async Task<bool> TryStartProcessingAsync(IdempotencyDto idempotencyDto)
        {
            try
            {
                const string sql = @"
                INSERT INTO EventBus.EventProcessing
                    (EventId, CorrelationId, EventType, Status)
                VALUES
                    (@EventId, @CorrelationId, @EventType, @Status)";

                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@EventId", idempotencyDto.EventId);
                command.Parameters.AddWithValue("@CorrelationId", idempotencyDto.CorrelationId);
                command.Parameters.AddWithValue("@EventType", idempotencyDto.EventType);
                command.Parameters.AddWithValue("@Status", (int)ProcessingStatus.Processing);
                await command.ExecuteNonQueryAsync();

                return true;
            }
            catch (SqlException sqlex) when (sqlex.Number == 2627 || sqlex.Number == 2601) //codigo de error para violación de clave única
            {
                _logger.LogInformation("El evento {EventType} con el Id {EventId} ya fue registrado.", idempotencyDto.EventType,idempotencyDto.EventId);
                return false;
            }
        }
    }
}
