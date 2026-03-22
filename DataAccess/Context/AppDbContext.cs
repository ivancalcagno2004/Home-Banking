using HomeBanking.Data.Configurations;
using Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Context
{
    public class AppDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<TransactionCategoryMap> TransactionCategoryMaps { get; set; }
        public DbSet<ServicePayment> ServicePayments { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // FLUENT API

            // Account
            modelBuilder.Entity<Account>()
                .Property(a => a.Balance)
                .HasPrecision(18, 2);

            // Transaction
            modelBuilder.Entity<Transaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Transaction>()
               .HasOne(t => t.FromAccount)
               .WithMany(a => a.TransactionsFrom)
               .HasForeignKey(t => t.FromAccountId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ToAccount)
                .WithMany(a => a.TransactionsTo)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // PK compuesta
            modelBuilder.Entity<TransactionCategoryMap>()
                .HasKey(x => new { x.TransactionId, x.TransactionCategoryId });

            // Relaciones
            modelBuilder.Entity<TransactionCategoryMap>()
                .HasOne(x => x.Transaction)
                .WithMany(t => t.TransactionCategoryMaps)
                .HasForeignKey(x => x.TransactionId);

            modelBuilder.Entity<TransactionCategoryMap>()
                .HasOne(x => x.TransactionCategory)
                .WithMany(c => c.TransactionCategoryMaps)
                .HasForeignKey(x => x.TransactionCategoryId);


            // ServicePayment
            modelBuilder.Entity<ServicePayment>(entity =>
            {
                entity.HasOne(p => p.User)
                      .WithMany()
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Category)
                      .WithMany()
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.ApplyConfiguration(new ServicePaymentConfiguration());

            // Aplicar configuraciones externas
            modelBuilder.ApplyConfiguration(new ServicePaymentConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());       
            modelBuilder.ApplyConfiguration(new AccountConfiguration());    
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionCategoryMapConfiguration());

            // Compatibilidad con SQLite
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var decimalProperties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(decimal));

                    foreach (var property in decimalProperties)
                    {
                        modelBuilder.Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion<double>();
                    }

                    foreach (var property in entityType.GetProperties())
                    {
                        var defaultValueSql = property.GetDefaultValueSql();
                        if (defaultValueSql != null &&
                            defaultValueSql.Contains("GETDATE()", StringComparison.OrdinalIgnoreCase))
                        {
                            property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                        }
                    }

                    var table = entityType.GetTableName();
                    if (table != null) modelBuilder.Entity(entityType.Name).ToTable(table);
                }
            }
        }
    }
}
