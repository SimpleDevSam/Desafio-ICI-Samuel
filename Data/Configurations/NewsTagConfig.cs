using Desafio_ICI_Samuel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio_ICI_Samuel.Data.Configurations;

public class NewsTagConfig : IEntityTypeConfiguration<NewsTag>
{
    public void Configure(EntityTypeBuilder<NewsTag> b)
    {
        b.ToTable("NewsTags");
        b.HasKey(x => new { x.NewsId, x.TagId });

        b.HasOne(x => x.News)
            .WithMany(n => n.NewsTags)
            .HasForeignKey(x => x.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.Tag)
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
