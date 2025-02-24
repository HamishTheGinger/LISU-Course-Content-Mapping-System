using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class WeekGraduateAttributes
    {
        [ForeignKey("Week")]
        public int WeekId { get; set; }
        public required Week Week { get; set; }

        [ForeignKey("GraduateAttribute")]
        public int GraduateAttributeId { get; set; }
        public required GraduateAttribute GraduateAttribute { get; set; }
    }
}
