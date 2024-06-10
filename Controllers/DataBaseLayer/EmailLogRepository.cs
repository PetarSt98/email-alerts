using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using email_alerts.DataBaseLayer.Data;


namespace email_alerts.Data
{
    public class EmailLogRepository
    {
        private readonly string _connectionString;

        public EmailLogRepository(IConfiguration configuration) // ILogger<EmailLogRepository> logger
        {
            //_logger = logger;

            var connectionString = configuration.GetConnectionString("MSSQLConnection");
            var username = configuration["DB_USERNAME"];
            var password = configuration["DB_PASSWORD"];

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                //_logger.LogError("DB_USERNAME or DB_PASSWORD configuration values are not set.");
                throw new InvalidOperationException("Database credentials are not set.");
            }

            
            _connectionString = connectionString
                .Replace("{DB_USERNAME}", username)
                .Replace("{DB_PASSWORD}", password);
            Console.WriteLine(_connectionString);
            _connectionString = configuration.GetConnectionString("MSSQLConnection")
                .Replace("{DB_USERNAME}", Environment.GetEnvironmentVariable("DB_USERNAME"))
                .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD"));
            Console.WriteLine(_connectionString);
            Console.WriteLine(Environment.GetEnvironmentVariable("DB_USERNAME"));


            //_logger.LogInformation("Connection string successfully configured with user secrets.");
        }

        public IEnumerable<EmailLog> GetEmailLogs()
        {
            var emailLogs = new List<EmailLog>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.EmailLog", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var emailLog = new EmailLog
                            {
                                ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                QueryID = reader.GetInt32(reader.GetOrdinal("QueryID")),
                                EMail = reader.GetString(reader.GetOrdinal("EMail")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                PCName = reader.GetString(reader.GetOrdinal("PCName")),
                                Info = reader.IsDBNull(reader.GetOrdinal("Info")) ? null : reader.GetString(reader.GetOrdinal("Info")),
                                SentStatus = reader.IsDBNull(reader.GetOrdinal("SentStatus")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SentStatus")),
                                SessionID = reader.IsDBNull(reader.GetOrdinal("SessionID")) ? (Guid?)null : reader.GetGuid(reader.GetOrdinal("SessionID"))
                            };
                            emailLogs.Add(emailLog);
                        }
                    }
                }
            }

            return emailLogs;
        }
    }
}
