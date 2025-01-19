using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class WeekActivities
    {
        [Key]
        public int WeekActivityId { get; set; }
        
        [ForeignKey("Week")]
        public int WeekId { get; set; }
        public required Week Week { get; set; }

        [ForeignKey("Activities")]
        public int ActivitiesId { get; set; }
        public required Activities Activities { get; set; }
        
        public string TaskTitle { get; set; }
        public string TaskStaff { get; set; }
        public TimeOnly TaskTime { get; set; }
        public string TasksStatus { get; set; }
        
        [ForeignKey("LearningType")]
        public int LearningTypeId { get; set; }
        public LearningType LearningType { get; set; }
        
        public string TaskLocation { get; set; }

        public string TaskApproach { get; set; }
        
        public int TaskOrder { get; set; }

    }
}