using System.ComponentModel.DataAnnotations;

namespace WebApiSample.Entities;

public class Role:BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    [Required]
    [StringLength(100)]
    public string Description { get; set; }
}