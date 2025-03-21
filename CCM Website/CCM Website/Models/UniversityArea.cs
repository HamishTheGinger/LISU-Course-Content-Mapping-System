using System.ComponentModel.DataAnnotations;

namespace CCM_Website.Models
{
    public class UniversityArea
    {
        [Key]
        public int AreaId { get; set; }

        public required string AreaName { get; set; }
    }
}
