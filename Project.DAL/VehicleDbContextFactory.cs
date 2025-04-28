using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Project.DAL
{
    public class VehicleDbContextFactory : IDesignTimeDbContextFactory<VehicleDbContext>
    {
        public VehicleDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<VehicleDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=DESKTOP-8MAAE36;Database=VehicleManagement;Trusted_Connection=True;TrustServerCertificate=True;");

            return new VehicleDbContext(optionsBuilder.Options);
        }
    }
}