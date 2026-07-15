using CorporateEvents.Abstractions.Configuration;
using CorporateEvents.Abstractions.Contracts;
using CorporateEvents.Abstractions.Dtos;
using CorporateEvents.Abstractions.Enum;
using CorporateEvents.Persistence.SqlServer.Idempotency;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateEvents.Persistence.SqlServer.Metrics
{
    public class SqlMetricsService : IMetricsService
    {
        private readonly string _connectionString;
        public SqlMetricsService(IOptions<EventBusOptions> options)
        {
            _connectionString = options.Value.ConnectionString;
        }
        public async Task<MetricsDto> GetMetricsAsync()
        {
            const string sql = @"
        SELECT
            COUNT(*) AS PublishedEvents,
            SUM(CASE WHEN Status = @Processing THEN 1 ELSE 0 END) AS ProcessingEvents,
            SUM(CASE WHEN Status = @Completed THEN 1 ELSE 0 END) AS CompletedEvents,
            SUM(CASE WHEN Status = @Failed THEN 1 ELSE 0 END) AS FailedEvents
        FROM EventBus.EventProcessing;";

                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                await using var command = new SqlCommand(sql, connection);

                command.Parameters.AddWithValue("@Processing", (int)ProcessingStatus.Processing);
                command.Parameters.AddWithValue("@Completed", (int)ProcessingStatus.Completed);
                command.Parameters.AddWithValue("@Failed", (int)ProcessingStatus.Failed);

                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new MetricsDto
                    {
                        PublishedEvents = reader.GetInt32(reader.GetOrdinal("PublishedEvents")),
                        ProcessedEvents = reader.GetInt32(reader.GetOrdinal("CompletedEvents")),
                        FailedEvents = reader.GetInt32(reader.GetOrdinal("FailedEvents"))
                    };
            }

            return new MetricsDto();
        }
    }
}
