namespace GarageOperationsManagementSystem.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Position { get; set; }

        public decimal Salary { get; set; }

        public DateTime WorkingSince { get; set; }

        public int GarageId { get; set; }

        public Garage Garage { get; set; }
    }
}
