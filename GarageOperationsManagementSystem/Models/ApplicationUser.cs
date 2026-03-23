using Microsoft.AspNetCore.Identity;

namespace GarageOperationsManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        //public int? GarageId { get; set; }
    }
}