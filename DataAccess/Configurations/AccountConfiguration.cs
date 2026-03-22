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
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(a => a.AccountId);

            builder.Property(a => a.CBU)
                .IsRequired()
                .HasMaxLength(22);

            builder.Property(a => a.Alias)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.Balance)
                .HasPrecision(18, 2)
                .IsRequired();
                
            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();
        }
    }
}
