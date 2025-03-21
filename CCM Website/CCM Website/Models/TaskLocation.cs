using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class TaskLocation
    {
        [Key]
        public int LocationId { get; set; }

        public required string LocationName { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? LocationColour { get; set; }

        [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
        public string? LocationTextColour { get; set; }
    }
}
