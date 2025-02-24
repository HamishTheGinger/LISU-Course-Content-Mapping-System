using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models
{
    public class Week
    {
        [Key]
        public int WeekId { get; set; }

        public int WeekNumber { get; set; }

        // Navigation Properties
        [ForeignKey("Workbook")]
        public int WorkbookId { get; set; }
        public required Workbook Workbook { get; set; }
        public ICollection<WeekActivities>? WeekActivities { get; set; }
        public ICollection<WeekGraduateAttributes>? WeekGraduateAttributes { get; set; }
    }
}
