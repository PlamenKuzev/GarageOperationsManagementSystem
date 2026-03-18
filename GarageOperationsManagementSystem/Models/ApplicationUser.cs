using Microsoft.AspNetCore.Identity;

namespace GarageOperationsManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
