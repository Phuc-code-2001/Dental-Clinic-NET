using DataLayer.Schemas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataContexts
{
    public class AppDbContext : DbContext
    {
        private static AppDbContext Instance { get; set; }

        public static AppDbContext GetInstance => Instance ?? (Instance = new AppDbContext());

        public AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        private static string host = "ec2-54-76-43-89.eu-west-1.compute.amazonaws.com";
        private static string user = "kuxjgkqbsvckvi";
        private static string database = "d1kev7iaq1v931";
        private static string password = "7ce3524d49aef95125a4ae81167e8da0bc7c0d0a446da9cdda49b84508f85a3e";

        private static string CONNECTION => $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(CONNECTION);
            }
        }

        public DbSet<GroupMember> GroupMembers { get; set; }

        

    }
}
