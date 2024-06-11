using System.Data.SqlClient;
using email_alerts.Models.EmailAlerts;
using Microsoft.EntityFrameworkCore;
using email_alerts.Data.Contexts;


namespace email_alerts.Data.Repositories
{
    public class EmailLogRepository
    {
        private readonly string _connectionString;
        private readonly EmailAlertsContext _context;

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
                .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD")) + ";TrustServerCertificate=True";
            Console.WriteLine(_connectionString);
            Console.WriteLine(Environment.GetEnvironmentVariable("DB_USERNAME"));


            //_logger.LogInformation("Connection string successfully configured with user secrets.");
            var options = new DbContextOptionsBuilder<EmailAlertsContext>()
                .UseSqlServer(_connectionString)
                .Options;


            _context = new EmailAlertsContext(options);
        }

        public IEnumerable<EmailLog> GetEmailLogs()
        {
            ///
            try
            {
                var firstEmailLog = _context.EmailLogs
                    .Include(e => e.Session)
                    .Include(e => e.Query)
                    .FirstOrDefault();
                if (firstEmailLog != null)
                {
                    Console.WriteLine($"First EmailLog ID: {firstEmailLog.ID}");
                    Console.WriteLine($"Email: {firstEmailLog.EMail}");
                    Console.WriteLine($"Session UserName: {firstEmailLog.Session?.UserName}");
                    Console.WriteLine($"Query Text: {firstEmailLog.Query?.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ///

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
