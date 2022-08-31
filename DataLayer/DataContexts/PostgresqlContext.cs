using DataLayer.Schemas;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataContexts
{
    public class PostgresqlContext : AppDbContext
    {
        private static string host = "ec2-52-208-164-5.eu-west-1.compute.amazonaws.com";
        private static string user = "dbq3cc6ivss5qc";
        private static string database = "gtqismcjwdqwpw";
        private static string password = "f37c39d5c6309ea22833a73b56dd84714ec81342a22addb111718068adaf5b31";

        private static string CONNECTION => $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";

        public PostgresqlContext() { }

        public PostgresqlContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(CONNECTION);
            }
        }

    }
}
