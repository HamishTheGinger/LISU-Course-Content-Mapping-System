using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class LearningPlatform
    {
        [Key]
        public int PlatformId { get; set; }

        public required string PlatformName { get; set; }

        // Navigation Properties
        [Required]
        public ICollection<Workbook> Workbooks { get; set; }
        
        [Required]
        public required ICollection<LearningPlatformActivities> LearningPlatformActivities { get; set; }
    }
}