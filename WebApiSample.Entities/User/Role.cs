
using Microsoft.AspNetCore.Identity;

namespace WebApiSample.Entities;

public class Role:IdentityRole<int>,IEntity
{
    public string Description { get; set; }
}