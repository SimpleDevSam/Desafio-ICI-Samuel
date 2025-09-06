using Desafio_ICI_Samuel.Domain;
using Desafio_ICI_Samuel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Desafio_ICI_Samuel.Data.Configurations;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> u)
    {
        u.ToTable("Users");
        u.HasKey(t => t.Id);

        u.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(250);

        u.Property(t => t.Password)
              .IsRequired()
              .HasMaxLength(250);

        u.Property(t => t.Password)
            .IsRequired()
            .HasMaxLength(250);

        u.Property(t => t.Email)
           .IsRequired()
           .HasMaxLength(250);

        u.HasData(
           new User { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Password = "Passw0rd!" },
           new User { Id = 2, Name = "Bob Smith", Email = "bob@example.com", Password = "Passw0rd!" },
           new User { Id = 3, Name = "Carol Lee", Email = "carol@example.com", Password = "Passw0rd!" }
       );
    }
}
