using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Infrastructure
{
    [DisallowConcurrentExecution]
    public class GetDataBackgroundJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<GetDataBackgroundJob> _logger;

        public GetDataBackgroundJob(IConfiguration configuration, ILogger<GetDataBackgroundJob> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var connectionString = _configuration.GetConnectionString("Dev");
            
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetLogsAndMessages", connection))
                    {
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            _logger.LogInformation("Logs Table:");

                            while (await reader.ReadAsync())
                            {
                                var logId = reader["Id"];
                                var message = reader["Username"];
                                var logTime = reader["Description"];
                                _logger.LogInformation($"LogId: {logId}, Message: {message}, LogTime: {logTime}");
                            }

                            // Move to the next result set (Messages Table)
                            if (await reader.NextResultAsync())
                            {
                                _logger.LogInformation("\nMessages Table:");

                                // Read the second result set (Messages Table)
                                while (await reader.ReadAsync())
                                {
                                    var messageId = reader["Id"];
                                    var senderUserName = reader["SenderUserName"];
                                    var content = reader["Text"];
                                    var messageDate = reader["CreatedAt"];
                                    _logger.LogInformation($"MessageId: {messageId}, Content: {content}, Sender: {senderUserName}, MessageDate: {messageDate}");
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation("Stored procedure executed successfully.");

            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while executing the stored procedure: {ex.Message}");
            }
        }
    }
}
