using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {
    public class TaskLocation {
        [Key]
        public int LocationId { get; set; }

        public required string LocationName { get; set; }
    }
}