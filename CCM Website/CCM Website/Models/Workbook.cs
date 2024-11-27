using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class Workbook
    {
        [Key]
        public int WorkbookId { get; set; }

        public required string CourseName { get; set; }
        public required string CourseLead { get; set; }
        public int CourseLength { get; set; }
        public DateTime LastEdited { get; set; }

        // Navigation Properties
        [ForeignKey("LearningPlatform")]
        public int PlatformId { get; set; }
        public required ICollection<Week> Weeks { get; set; }

        [ForeignKey("LearningPlatform")]
        public int LearningPlatformId { get; set; }
        public required LearningPlatform LearningPlatform { get; set; }
    }
}