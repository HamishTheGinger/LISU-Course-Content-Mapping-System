using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class LearningPlatformActivities
    {
        [ForeignKey("LearningPlatform")]
        public int LearningPlatformId { get; set; }
        public required LearningPlatform LearningPlatform { get; set; }

        [ForeignKey("Activities")]
        public int ActivitiesId { get; set; }
        public required Activities Activities { get; set; }
    }
}
