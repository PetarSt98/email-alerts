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

        public IEnumerable<EmailLog> GetEmailLogsByQueryId(int queryId)
        {
            var emailLogs = new List<EmailLog>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.EmailLog WHERE QueryID = @queryId", connection))
                {
                    command.Parameters.AddWithValue("@queryId", queryId);
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
                                PCName = reader.IsDBNull(reader.GetOrdinal("PCName")) ? null : reader.GetString(reader.GetOrdinal("PCName")),
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

        public void DeleteQuery(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("DELETE FROM dbo.Query WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
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

        public int GetTotalEmailLogsCount(int queryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo.EmailLog WHERE QueryID = @queryId", connection))
                {
                    command.Parameters.AddWithValue("@queryId", queryId);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public IEnumerable<EmailLog> GetEmailLogsByQueryId(int queryId, int page, int pageSize)
        {
            var emailLogs = new List<EmailLog>();
            var offset = (page - 1) * pageSize;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.EmailLog WHERE QueryID = @queryId ORDER BY ID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
                {
                    command.Parameters.AddWithValue("@queryId", queryId);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
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
                                PCName = reader.IsDBNull(reader.GetOrdinal("PCName")) ? null : reader.GetString(reader.GetOrdinal("PCName")),
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


        public int GetTotalQueriesCount()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo.Query", connection))
                {
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public IEnumerable<Query> GetQueriesToDisplay(int page, int pageSize)
        {
            var queries = new List<Query>();
            var offset = (page - 1) * pageSize;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.Query ORDER BY id OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
                {
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
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

        public List<Dictionary<string, object>> ExecuteQuery(string queryText)
        {
            var results = new List<Dictionary<string, object>>();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(queryText, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }
                                results.Add(row);
                            }
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                results.Clear();
                results.Add(new Dictionary<string, object> { {"Error", ex.Message } });
            }

            return results;
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

        public int GetTotalEmailLogsCountByStatus(int queryId, int status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT COUNT(*) FROM dbo.EmailLog WHERE QueryID = @queryId AND SentStatus = @status", connection))
                {
                    command.Parameters.AddWithValue("@queryId", queryId);
                    command.Parameters.AddWithValue("@status", status);
                    return (int)command.ExecuteScalar();
                }
            }
        }

        public IEnumerable<EmailLog> GetEmailLogsByQueryIdAndStatus(int queryId, int status, int page, int pageSize)
        {
            var emailLogs = new List<EmailLog>();
            var offset = (page - 1) * pageSize;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand("SELECT * FROM dbo.EmailLog WHERE QueryID = @queryId AND SentStatus = @status ORDER BY ID OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY", connection))
                {
                    command.Parameters.AddWithValue("@queryId", queryId);
                    command.Parameters.AddWithValue("@status", status);
                    command.Parameters.AddWithValue("@Offset", offset);
                    command.Parameters.AddWithValue("@PageSize", pageSize);
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
                                PCName = reader.IsDBNull(reader.GetOrdinal("PCName")) ? null : reader.GetString(reader.GetOrdinal("PCName")),
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
