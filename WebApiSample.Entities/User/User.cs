using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApiSample.Entities;

public class User : IdentityUser<int> , IEntity
{
    public User()
    {
        IsActive = true;
    }

    
    public string FullName { get; set; }
    public int Age { get; set; }
    public GenderType Gender { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset? LastLoginDate { get; set; }
    public ICollection<Post>? Posts { get; set; }
}

public enum GenderType
{
    [Display(Name = "مرد")] Male = 1,
    [Display(Name = "زن")] Female = 2
}
