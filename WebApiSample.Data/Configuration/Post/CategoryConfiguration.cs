using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiSample.Entities;

namespace WebApiSample.Data.Configuration;

public class CategoryConfiguration:IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
        builder.HasOne(p => p.ParentCategory)
            .WithMany(c => c.ChildCategories)
            .HasForeignKey(p => p.ParentCategoryId);
    }
}