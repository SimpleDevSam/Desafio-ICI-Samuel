using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio_ICI_Samuel.Data.Configurations;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> b)
    {
        b.ToTable("Tags");
        b.HasKey(t => t.Id);

        b.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(100);

        //Deixar index ou não? Mostrar que possuo esse conhecimento porém não é aplicávle a um projeto de final de semana.
        b.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Nome");
    }
}