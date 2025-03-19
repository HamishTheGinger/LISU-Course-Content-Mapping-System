using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class TaskProgressStatus
    {
        [Key]
        public int StatusId { get; set; }

        public required string StatusName { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? StatusColour { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? StatusTextColour { get; set; }
    }
}
