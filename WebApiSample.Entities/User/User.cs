using System.ComponentModel.DataAnnotations;

namespace WebApiSample.Entities;

public class User : BaseEntity
{
    public User()
    {
        IsActive = true;
        SecurityStamp = Guid.NewGuid();
    }

    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
    public Guid SecurityStamp { get; set; }
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
