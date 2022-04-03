using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable(nameof(User)).HasKey(item => item.Email);
            builder.Property(item => item.Email).IsRequired();
            builder.Property(item => item.PasswordHash).IsRequired();
        }
    }
}
