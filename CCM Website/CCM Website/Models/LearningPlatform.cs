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
        public ICollection<Workbook>? Workbooks { get; set; }
        public ICollection<LearningPlatformActivities>? LearningPlatformActivities { get; set; }
    }
}
