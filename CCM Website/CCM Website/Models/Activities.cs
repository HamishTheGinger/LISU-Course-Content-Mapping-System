using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {

    public class Activities {
        
        [Key]
        public int ActivityId { get; set; }
        public required string ActivityName { get; set; }
        
        // Navigation Propeties
        public ICollection<LearningPlatformActivities>? LearningPlatformActivities { get; set; }
        public ICollection<WeekActivities>? WeekActivities { get; set; }
    }
}