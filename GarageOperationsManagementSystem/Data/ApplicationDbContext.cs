using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Garage> Garages { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Owner> Owners { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<RepairOrder> RepairOrders { get; set; }



    }
}
