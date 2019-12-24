using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizedServer.Models
{
    public class DemoDbContext
    {
        public DbSet<RToken> RTokens { get; set; }

        //public DbSet<AToken> ATokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connStr = Path.Combine(Directory.GetCurrentDirectory(), "demo.db");

            optionsBuilder.UseSqlite($"Data Source={connStr}");
        }
    }
}
