using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {
    public class LearningApproach {
        [Key]
        public int ApproachId { get; set; }

        public required string ApproachName { get; set; }
    }
}