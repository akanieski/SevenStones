using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SevenStones.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SevenStones
{
    public class DataContext : DbContext
    {
        private readonly ILogger<DataContext> _logger;
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Fact> Facts { get; set; }
        public DbSet<SourceSystem> SourceSystems { get; set; }
        public DbSet<BlacklistEntry> BlacklistEntries { get; set; }

        public DataContext(DbContextOptions<DataContext> options, ILogger<DataContext> logger) : base(options) 
        {
            _logger = logger;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string sqlConnectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            optionsBuilder
                .UseSqlServer(sqlConnectionString, sqlOpt => sqlOpt.EnableRetryOnFailure())
                .UseLazyLoadingProxies();
        }
        public async Task Initialize()
        {
            RelationalDatabaseCreator databaseCreator =
            (RelationalDatabaseCreator)Database.GetService<IDatabaseCreator>();
            var s = databaseCreator.GenerateCreateScript();

            if (!(await BlacklistEntries.AnyAsync()))
            {
                // Initial Blacklist setup
                _logger.LogInformation($"Setting up initial blacklist entries..");
                BlacklistEntries.AddRange(new BlacklistEntry[]
                {
                    new BlacklistEntry()
                    {
                        Name = "Node Modules Found in Source",
                        Pattern = "**/node_modules"
                    },
                    new BlacklistEntry()
                    {
                        Name = "Bin/Debug Found in Source",
                        Pattern = "**/bin/debug"
                    },
                    new BlacklistEntry()
                    {
                        Name = "Bin/Release Found in Source",
                        Pattern = "**/bin/release"
                    },
                    new BlacklistEntry()
                    {
                        Name = "Executable Found in Source",
                        Pattern = "**/*.exe"
                    },
                    new BlacklistEntry()
                    {
                        Name = "DLL Found in Source",
                        Pattern = "**/*.dll"
                    },
                    new BlacklistEntry()
                    {
                        Name = "Library Found in Source",
                        Pattern = "**/*.so"
                    }
                });
                await SaveChangesAsync();


            }
        }
    }
}
