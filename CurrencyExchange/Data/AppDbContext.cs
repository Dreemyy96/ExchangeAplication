using CurrencyExchange.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        public DbSet<Application> Applications { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base (options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CurrencyRate>()
            .HasKey(cr => cr.Id);

            builder.Entity<Question>()
            .HasKey(cr => cr.Id);

            builder.Entity<Application>()
            .HasOne(a => a.Bank)
            .WithMany()
            .HasForeignKey(a => a.BankId);

            builder.Entity<TransferRequest>()
            .HasOne(a => a.Bank)
            .WithMany()
            .HasForeignKey(a => a.BankId);
        }
    }
}
