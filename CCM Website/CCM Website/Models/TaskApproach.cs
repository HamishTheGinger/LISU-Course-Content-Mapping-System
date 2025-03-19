using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class TaskApproach
    {
        [Key]
        public int ApproachId { get; set; }

        public required string ApproachName { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? ApproachColour { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? ApproachTextColour { get; set; }
    }
}
