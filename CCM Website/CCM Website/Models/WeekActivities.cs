using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class WeekActivities
    {
        [Key]
        public int WeekActivityId { get; set; }
        public required string TaskTitle { get; set; }
        public required string TaskStaff { get; set; }
        public required TimeOnly TaskTime { get; set; }
        public required string TasksStatus { get; set; }
        public required string TaskLocation { get; set; }
        public required string TaskApproach { get; set; }
        public required int TaskOrder { get; set; }

        // Navigation Propeties
        [ForeignKey("Week")]
        public int WeekId { get; set; }
        public required Week Week { get; set; }

        [ForeignKey("Activities")]
        public int ActivitiesId { get; set; }
        public required Activities Activities { get; set; }

        [ForeignKey("LearningType")]
        public int LearningTypeId { get; set; }
        public required LearningType LearningType { get; set; }
    }
}