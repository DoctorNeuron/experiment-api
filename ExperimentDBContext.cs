using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExperimentAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace ExperimentAPI
{
    public class ExperimentDBContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var baseModel = typeof(BaseEntity);
            var entities = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsSubclassOf(baseModel))
                .ToList();

            foreach (var e in entities) modelBuilder.Entity(e);
        }
    }
}