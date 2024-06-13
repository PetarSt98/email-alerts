using System.Data.SqlClient;
using email_alerts.Models.EmailAlerts;
using Microsoft.EntityFrameworkCore;
using email_alerts.Data.Contexts;


namespace email_alerts.Data.Repositories
{
    public class EmailAlertRepository
    {
        private readonly string _connectionString;
        private readonly EmailAlertsContext _context;

        public EmailAlertRepository(IConfiguration configuration) // ILogger<EmailLogRepository> logger
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
            _connectionString = configuration.GetConnectionString("MSSQLConnection")
                .Replace("{DB_USERNAME}", Environment.GetEnvironmentVariable("DB_USERNAME"))
                .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD")) + ";TrustServerCertificate=True";


            //_logger.LogInformation("Connection string successfully configured with user secrets.");
            var options = new DbContextOptionsBuilder<EmailAlertsContext>()
                .UseSqlServer(_connectionString)
                .Options;


            _context = new EmailAlertsContext(options);
        }

        public IEnumerable<Query> GetAllQueriesToDisplay()
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

            var queries = new List<Query>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.Query", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var query = new Query
                            {
                                id = reader.GetInt32(reader.GetOrdinal("id")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                            };
                            queries.Add(query);
                        }
                    }
                }
            }

            return queries;
        }

        public void UpdateQuery(Query query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("UPDATE dbo.Query SET Description = @Description, Timeout = @Timeout, Period = @Period, Active = @Active, Text = @Text, Subject = @Subject, ReceiverType = @ReceiverType, MessageFormat = @MessageFormat, Body = @Body WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@Description", query.Description);
                    command.Parameters.AddWithValue("@Timeout", query.Timeout);
                    command.Parameters.AddWithValue("@Period", query.Period);
                    command.Parameters.AddWithValue("@Active", query.Active);
                    command.Parameters.AddWithValue("@Text", query.Text);
                    command.Parameters.AddWithValue("@Subject", query.Subject);
                    command.Parameters.AddWithValue("@ReceiverType", query.ReceiverType);
                    command.Parameters.AddWithValue("@MessageFormat", query.MessageFormat);
                    command.Parameters.AddWithValue("@Body", query.Body);
                    command.Parameters.AddWithValue("@id", query.id);

                    command.ExecuteNonQuery();
                }
            }
        }


        public Query GetQueryById(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.Query WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                return new Query
                                {
                                    id = reader.GetInt32(reader.GetOrdinal("id")),
                                    Text = reader.GetString(reader.GetOrdinal("Text")),
                                    Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description")),
                                    Active = reader.GetBoolean(reader.GetOrdinal("Active")),
                                    Subject = reader.IsDBNull(reader.GetOrdinal("Subject")) ? null : reader.GetString(reader.GetOrdinal("Subject")),
                                    Body = reader.GetString(reader.GetOrdinal("Body")),
                                    Period = reader.IsDBNull(reader.GetOrdinal("Period")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Period")),
                                    LegacyMsg = reader.IsDBNull(reader.GetOrdinal("LegacyMsg")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("LegacyMsg")),
                                    Timeout = reader.IsDBNull(reader.GetOrdinal("Timeout")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Timeout")),
                                    TemplateID = reader.IsDBNull(reader.GetOrdinal("TemplateID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TemplateID")),
                                    TemplateParameters = reader.IsDBNull(reader.GetOrdinal("TemplateParameters")) ? null : reader.GetString(reader.GetOrdinal("TemplateParameters")),
                                    QueryType = reader.IsDBNull(reader.GetOrdinal("QueryType")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("QueryType")),
                                    ReceiverType = reader.IsDBNull(reader.GetOrdinal("ReceiverType")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ReceiverType")),
                                    MessageFormat = reader.GetInt32(reader.GetOrdinal("MessageFormat"))
                                };
                            }
                            catch (Exception ex)
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public void AddQuery(Query query)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("INSERT INTO dbo.Query (Description, Timeout, Period, Active, Text, Subject, ReceiverType, MessageFormat, Body) VALUES (@Description, @Timeout, @Period, @Active, @Text, @Subject, @ReceiverType, @MessageFormat, @Body)", connection))
                {
                    command.Parameters.AddWithValue("@Description", query.Description);
                    command.Parameters.AddWithValue("@Timeout", query.Timeout);
                    command.Parameters.AddWithValue("@Period", query.Period);
                    command.Parameters.AddWithValue("@Active", query.Active);
                    command.Parameters.AddWithValue("@Text", query.Text);
                    command.Parameters.AddWithValue("@Subject", query.Subject);
                    command.Parameters.AddWithValue("@ReceiverType", query.ReceiverType);
                    command.Parameters.AddWithValue("@MessageFormat", query.MessageFormat);
                    command.Parameters.AddWithValue("@Body", query.Body);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
