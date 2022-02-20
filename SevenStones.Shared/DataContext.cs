using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenStones.Models;

namespace SevenStones
{
    public class DataContext : DbContext
    {
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Fact> Facts { get; set; }
        public DbSet<SourceSystem> SourceSystems { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string sqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");

            optionsBuilder
            .UseSqlServer(sqlConnectionString, sqlOpt => sqlOpt.EnableRetryOnFailure())
            .UseLazyLoadingProxies()
                .LogTo(msg =>
                {
                    // Do nothing
                }, Microsoft.Extensions.Logging.LogLevel.Error);
        }
    }
}
