using CardholderApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace CardholderApi.Persistence
{
    public class CardholderDbContext(DbContextOptions<CardholderDbContext> options) : DbContext(options)
    {
        public DbSet<Cardholder> Cardholders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cardholder>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.Address)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(30);

                entity.Property(e => e.TransactionCount)
                      .IsRequired();
            });
        }
    }
}
