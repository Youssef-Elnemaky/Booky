using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookyWeb.Models.Data.Config
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Name).HasColumnType("VARCHAR(100)");

            builder.HasData(
            [
                new Category {Id = 1, Name = "Scientific", DisplayOrder = 1},
                new Category {Id = 2, Name = "Comedy", DisplayOrder = 2},
                new Category {Id = 3, Name = "Romance", DisplayOrder = 3},
            ]);
        }
    }
}
