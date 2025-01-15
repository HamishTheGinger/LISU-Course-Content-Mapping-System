using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class WeekActivities
    {
        [ForeignKey("Week")]
        public int WeekId { get; set; }
        public required Week Week { get; set; }

        [ForeignKey("Activities")]
        public int ActivitiesId { get; set; }
        public required Activities Activities { get; set; }
    }
}