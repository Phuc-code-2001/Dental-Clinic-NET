using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataContexts
{
    public class SqlServerContext : AppDbContext
    {
        private static string server = "PHUCHT\\SQLEXPRESS";
        private static string database = "DentalClinicApp";
        private static string uid = "sa";
        private static string password = "123456";

        private static string CONNECTION => $"Server={server};database={database};uid={uid};pwd={password}";

        public SqlServerContext () { }

        public SqlServerContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(CONNECTION);
            }
        }
    }
}
