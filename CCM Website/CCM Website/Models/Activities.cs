using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {

    public class Activities {
        
        [Key]
        public int ActivityId { get; set; }
        public required string ActivityName { get; set; }
        
        // Navigation Propeties
        public required ICollection<LearningPlatformActivities> LearningPlatformActivities { get; set; }
        public required ICollection<WeekActivities> WeekActivities { get; set; }
    }
}