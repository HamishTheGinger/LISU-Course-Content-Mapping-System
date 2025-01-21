using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {
    public class GraduateAttribute {
        [Key]
        public int AttributeId { get; set; }

        public required string AttributeName { get; set; }

        // Navigation Propeties
        public ICollection<WeekGraduateAttributes>? WeekGraduateAttributes { get; set; }
    }
}