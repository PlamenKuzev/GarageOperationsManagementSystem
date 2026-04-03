using GarageOperationsManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Tests
{
    internal static class TestDbContextFactory
    {
        public static ApplicationDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
