using Microsoft.EntityFrameworkCore;

namespace SignalR.API.Models
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=MSI;database=SignalR;integrated security=true;trusted_connection=true;encrypt=false");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
    }
}
