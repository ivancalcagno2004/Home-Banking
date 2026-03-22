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
    public class TransactionCategoryMapConfiguration : IEntityTypeConfiguration<TransactionCategoryMap>
    {
        public void Configure(EntityTypeBuilder<TransactionCategoryMap> builder)
        {
            builder.ToTable("TransactionCategoryMap");

            builder.HasKey(x => new { x.TransactionId, x.TransactionCategoryId });

            builder.HasOne(x => x.Transaction)
                   .WithMany(t => t.TransactionCategoryMaps)
                   .HasForeignKey(x => x.TransactionId);

            builder.HasOne(x => x.TransactionCategory)
                   .WithMany(c => c.TransactionCategoryMaps)
                   .HasForeignKey(x => x.TransactionCategoryId);
        }
    }
}
