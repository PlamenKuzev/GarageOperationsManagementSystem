using System.ComponentModel.DataAnnotations;

namespace GarageOperationsManagementSystem.ViewModels.Public
{
    public class RepairStatusLookupViewModel
    {
        [Required]
        [StringLength(32, MinimumLength = 3)]
        [Display(Name = "Issue code")]
        public string IssueCode { get; set; } = string.Empty;

        public bool NotFound { get; set; }

        public RepairStatusResultViewModel? Result { get; set; }
    }

    public class RepairStatusResultViewModel
    {
        public string IssueCode { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public decimal? RepairPrice { get; set; }
        public string GarageCity { get; set; } = string.Empty;
        public string GarageAddress { get; set; } = string.Empty;
        public string CarSummary { get; set; } = string.Empty;
    }
}

