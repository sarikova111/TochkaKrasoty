using Microsoft.EntityFrameworkCore;
using TochkaKrasoty.Models;

namespace TochkaKrasoty.Data
{
    public class SalonDbContext : DbContext
    {
        public SalonDbContext(DbContextOptions<SalonDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Master> Masters { get; set; }
        public DbSet<ServiceItem> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}