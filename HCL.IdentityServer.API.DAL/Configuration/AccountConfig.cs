using HCL.IdentityServer.API.DAL.Configuration.DataType;
using HCL.IdentityServer.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HCL.IdentityServer.API.DAL.Configuration
{
    public class AccountConfig : IEntityTypeConfiguration<Account>
    {
        public const string Table_name = "accounts";

        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable(Table_name);

            builder.HasKey(e => new { e.Id });
            builder.HasIndex(e => e.Login)
                .IsUnique();

            builder.Property(e => e.Id)
                   .HasColumnType(EntityDataTypes.Guid)
                   .HasColumnName("pk_account_id");

            builder.Property(e => e.Login)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("login");

            builder.Property(e => e.Password)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("password");

            builder.Property(e => e.Salt)
                   .HasColumnType(EntityDataTypes.Character_varying)
                   .HasColumnName("salt");

            builder.Property(e => e.CreateDate)
                   .HasColumnName("create_date");

            builder.Property(e => e.StatusAccount)
                   .HasColumnType(EntityDataTypes.Smallint)
                   .HasColumnName("status_account");

            builder.Property(e => e.Role)
                   .HasColumnType(EntityDataTypes.Smallint)
                   .HasColumnName("role");
        }
    }
}