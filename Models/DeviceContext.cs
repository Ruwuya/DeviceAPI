using Microsoft.EntityFrameworkCore;

namespace DeviceAPI.Models
{
    public class DeviceContext : DbContext
    {
        public DeviceContext(DbContextOptions<DeviceContext> options)
            : base(options)
        {
        }

        public DbSet<DeviceItem> DeviceItems { get; set; } = null!;
    }
}