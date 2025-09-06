using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio_ICI_Samuel.Data.Configurations;

public class NewsConfig : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> b)
    {
        b.ToTable("News");
        b.HasKey(n => n.Id);

        b.Property(n => n.Title).IsRequired().HasMaxLength(250);
        b.Property(n => n.Text).IsRequired().HasColumnType("TEXT");

        b.HasOne(n => n.User)
            .WithMany() 
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
