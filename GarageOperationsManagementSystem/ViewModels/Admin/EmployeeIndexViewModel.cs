namespace GarageOperationsManagementSystem.ViewModels.Admin
{
    public class EmployeeIndexViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime WorkingSince { get; set; }
        public string GarageName { get; set; } = string.Empty;
        public bool IsTrusted { get; set; }

        /// <summary>Null when the employee record has no linked ApplicationUser account.</summary>
        public string? LinkedEmail { get; set; }
    }
}
