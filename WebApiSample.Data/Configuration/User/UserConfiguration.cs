using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiSample.Entities;

namespace WebApiSample.Data.Configuration;

public class UserConfiguration:IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.UserName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.FullName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.PasswordHash).IsRequired().HasMaxLength(500);
    }
}
