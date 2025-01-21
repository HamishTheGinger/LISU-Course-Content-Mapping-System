using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCM_Website.Models;

public class LearningType {
    [Key]
    public int LearningTypeId { get; set; }
    
    public required string LearningTypeName { get; set; }
}