using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.TransactionId);

            builder.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(t => t.Status).IsRequired().HasMaxLength(50);

            builder.Property(t => t.Description).HasMaxLength(500);

            builder.Property(t => t.CreatedAt).HasDefaultValueSql("GETDATE()").IsRequired();

            builder.HasOne(t => t.FromAccount)
                   .WithMany(a => a.TransactionsFrom)
                   .HasForeignKey(t => t.FromAccountId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.ToAccount)
                   .WithMany(a => a.TransactionsTo)
                   .HasForeignKey(t => t.ToAccountId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
