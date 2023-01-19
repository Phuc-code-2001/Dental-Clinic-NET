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

        private static string host = "sql.bsite.net\\MSSQL2016";
        private static string user = "phucht2022_DentalClinicNET";
        private static string password = "12345678";
        private static string database = "phucht2022_DentalClinicNET";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = $"Server={host};Database={database};uid={user};pwd={password}";
                optionsBuilder.UseSqlServer(connectionString);
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
        public DbSet<AppointmentDocument> AppointmentsDocuments { get; set; }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserInChatBoxOfReception> UsersInChatBoxOfReception { get; set; }

        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

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
