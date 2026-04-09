using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBanking.Data.Context
{
    /// <summary>
    /// Factory de diseño para EF Core (migraciones).
    /// Permite crear un <see cref="AppDbContext"/> fuera del runtime de la app,
    /// usando la cadena de conexión definida en <see cref="DatabaseSecrets"/>.
    /// </summary>
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            builder.UseSqlServer(DatabaseSecrets.ConnectionStringAzure, sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            return new AppDbContext(builder.Options);
        }
    }
}
