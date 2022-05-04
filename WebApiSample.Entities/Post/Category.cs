using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApiSample.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
    
    public Category? ParentCategory { get; set; }
    public ICollection<Category>? ChildCategories { get; set; }
    public ICollection<Post>? Posts { get; set; }
}