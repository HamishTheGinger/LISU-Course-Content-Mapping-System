using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class WeekActivities
    {
        [Key]
        public int WeekActivityId { get; set; }
        public required string TaskTitle { get; set; }
        public required string TaskStaff { get; set; }

        [UIHint("TimeSpan")]
        public required TimeSpan TaskTime { get; set; }

        public required int TaskOrder { get; set; }

        [ForeignKey("TasksStatus")]
        public int TasksStatusId { get; set; }
        public required TaskProgressStatus TasksStatus { get; set; }

        [ForeignKey("TaskLocation")]
        public int TaskLocationId { get; set; }
        public required TaskLocation TaskLocation { get; set; }

        [ForeignKey("TaskApproach")]
        public int TaskApproachId { get; set; }
        public required TaskApproach TaskApproach { get; set; }

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
