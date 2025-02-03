using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models {
    public class TaskProgressStatus {
        [Key]
        public int StatusId { get; set; }

        public required string StatusName { get; set; }
    }
}