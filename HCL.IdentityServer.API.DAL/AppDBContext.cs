using HCL.IdentityServer.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HCL.IdentityServer.API.DAL
{
    public partial class AppDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }

        public void UpdateDatabase()
        {
            Database.EnsureDeleted();
            Database.Migrate();
        }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        public AppDBContext()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}