using DataLayer.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DataContexts
{
    public class AppDbContext : IdentityDbContext<BaseUser>
    {

        public AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        private static string host = "ec2-34-243-101-244.eu-west-1.compute.amazonaws.com";
        private static string user = "bphhmbwrjoskky";
        private static string database = "dbvkiobqsmietn";
        private static string password = "a9d3865a698ba684463ecdde6967134cd34299897dbdf60a02e3252df9358533";

        private static string CONNECTION => $"Host={host};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(CONNECTION);
            }
        }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<MediaFile> Medias { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Service> Services { get; set; }

        public DbSet<Doctor> Doctors { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public override int SaveChanges()
        {

            var entryEntities = ChangeTracker.Entries<BaseEntity>();

            foreach(var entry in entryEntities)
            {
                if(entry.State == EntityState.Modified)
                {
                    entry.Entity.LastTimeModified = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

    }
}
