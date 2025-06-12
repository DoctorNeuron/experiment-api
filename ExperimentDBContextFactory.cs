using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace ExperimentAPI
{
    public class ExperimentDBContextFactory : IDesignTimeDbContextFactory<ExperimentDBContext>
    {
        public ExperimentDBContext CreateDbContext(string[] args)
        {
            var conf = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .Build();
            return new ExperimentDBContext(
                new DbContextOptionsBuilder<ExperimentDBContext>()
                    .UseNpgsql(conf.GetConnectionString("Default"))
                    .Options
            );
        }
    }
}