using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models;

public class LearningType
{
    [Key]
    public int LearningTypeId { get; set; }

    public required string LearningTypeName { get; set; }

    [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
    public string? LearningTypeColour { get; set; }

    [RegularExpression("^#(?:[0-9a-fA-F]{3}){1,2}$", ErrorMessage = "Invalid hex colour code")]
    public string? LearningTypeTextColour { get; set; }

    public string? LearningTypeDescription { get; set; }
}
