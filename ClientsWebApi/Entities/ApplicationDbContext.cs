using ClientsWebApi.Services;
using ClientsWebApi.Settings;
using ClientsWebApi.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace ClientsWebApi.Entities
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Founder> Founders { get; set; } = null!;
        public DbSet<ContactInfo> ContactInfo { get; set; } = null!;
        public DbSet<Contract> Contracts { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;

        private readonly AutoAdminSettings _adminSettings;

        public ApplicationDbContext(DbContextOptions options, IOptions<AutoAdminSettings> adminSettings) : base(options)
        {
            _adminSettings = adminSettings.Value;

            ArgumentNullException.ThrowIfNull(_adminSettings);
            ArgumentNullException.ThrowIfNull(_adminSettings.Email);
            ArgumentNullException.ThrowIfNull(_adminSettings.Password);

            _adminSettings.Username ??= new(_adminSettings.Email.TakeWhile(ch => ch != '@').ToArray());
        }

        private void SetAuditedColumns()
        {
            var entitiesCreated = ChangeTracker
                .Entries()
                .Where(e => e.Entity is EntityBase
                         && e.State == EntityState.Added)
                .Select(e => e.Entity as EntityBase);

            var entitiesModified = ChangeTracker
                .Entries()
                .Where(e => e.Entity is EntityBase
                         && e.State == EntityState.Modified)
                .Select(x => x.Entity as EntityBase);

            foreach (var entity in entitiesCreated)
            {
                entity!.CreatedDate = DateTime.UtcNow;
            }

            foreach (var entity in entitiesModified)
            {
                entity!.ModifiedDate = DateTime.UtcNow;
            }
        }

        public override int SaveChanges()
        {
            SetAuditedColumns();

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditedColumns();

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Founder>()
                .Property(f => f.FullName)
                .HasComputedColumnSql("CONCAT(LastName, ' ', FirstName, ' ', Patronymic)");

            modelBuilder.Entity<Founder>()
                .HasIndex(f => f.TaxpayerIndividualNumber)
                .IsUnique();


            modelBuilder.Entity<Client>()
                .HasIndex(c => c.TaxpayerIndividualNumber)
                .IsUnique();

            modelBuilder.Entity<Client>()
                .HasIndex(c => c.AccountId)
                .IsUnique();


            modelBuilder.Entity<Contract>()
                .HasOne(contract => contract.Client)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
