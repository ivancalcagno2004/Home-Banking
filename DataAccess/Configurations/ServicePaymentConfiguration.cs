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
    public class ServicePaymentConfiguration : IEntityTypeConfiguration<ServicePayment>
    {
        public void Configure(EntityTypeBuilder<ServicePayment> builder)
        {
            builder.ToTable("ServicePayments");

            // Clave Primaria
            builder.HasKey(p => p.Id);

            // PROPIEDADES
            builder.Property(p => p.ServiceName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.ExpDate)
                   .IsRequired();

            // RELACIONES (Foreign Keys)

            // Relación con Usuario
            builder.HasOne(p => p.User)
                   .WithMany()
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade); // Si borro user, borro sus deudas

            // Relación con Categoría
            builder.HasOne(p => p.Category)
                   .WithMany()
                   .HasForeignKey(p => p.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
