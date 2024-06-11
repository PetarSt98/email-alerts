using Microsoft.EntityFrameworkCore;
using email_alerts.Models.EmailAlerts;


namespace email_alerts.Data.Contexts
{
    public class EmailAlertsContext : DbContext
    {
        public EmailAlertsContext(DbContextOptions<EmailAlertsContext> options)
            : base(options)
        {
        }

        public DbSet<EmailLog> EmailLogs { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<QueryLog> QueryLogs { get; set; }
        public DbSet<Query> Queries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EmailLog configuration
            modelBuilder.Entity<EmailLog>()
                .HasKey(e => e.ID);

            modelBuilder.Entity<EmailLog>()
                .HasOne(e => e.Session)
                .WithMany(s => s.EmailLog) // Assuming there's no collection of EmailLogs in Session
                .HasForeignKey(e => e.SessionID)
                .HasConstraintName("FK_EmailLog_Session");

            modelBuilder.Entity<EmailLog>()
                .HasOne(e => e.Query)
                .WithMany()
                .HasForeignKey(e => e.QueryID)
                .HasConstraintName("FK_EmailLog_Query");

            // Session configuration
            modelBuilder.Entity<Session>()
                .HasKey(s => s.id);

            modelBuilder.Entity<Session>()
                .Property(s => s.id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Session>()
                .HasMany(s => s.EmailLog)
                .WithOne(e => e.Session)
                .HasForeignKey(e => e.SessionID)
                .HasConstraintName("FK_EmailLog_Session");

            // QueryLog configuration
            modelBuilder.Entity<QueryLog>()
                .HasKey(q => q.id);

            modelBuilder.Entity<QueryLog>()
                .HasOne(q => q.Query)
                .WithMany() // Assuming no collection navigation property in Query
                .HasForeignKey(q => q.QueryID)
                .HasConstraintName("FK_QueryLog_Query");

            modelBuilder.Entity<QueryLog>()
                .HasOne(q => q.Session)
                .WithMany() // Assuming no collection navigation property in Session
                .HasForeignKey(q => q.SessionID)
                .HasConstraintName("FK_QueryLog_Session");

            // Query configuration
            modelBuilder.Entity<Query>()
                .HasKey(q => q.id);
        }
    }
}
