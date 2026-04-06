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
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            string connectionString = "Server=tcp:otrosv.database.windows.net,1433;Initial Catalog=tandil-bank;Persist Security Info=False;User ID=CloudSA66e855f8;Password=Cocodepapa318;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            builder.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
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
