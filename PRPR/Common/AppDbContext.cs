using Microsoft.EntityFrameworkCore;
using PRPR.ExReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRPR.Common
{
    public class AppDbContext : DbContext
    {
        public DbSet<ExSearchRecord> ExSearchRecords { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=prpr.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExSearchRecord>().HasKey(nameof(ExSearchRecord.Id));
        }
    }
}
