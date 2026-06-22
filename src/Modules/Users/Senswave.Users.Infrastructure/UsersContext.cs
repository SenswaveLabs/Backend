using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Senswave.Infrastructure.Extensions;
using Senswave.Infrastructure.Persistence;
using Senswave.Users.Domain.Entity;

namespace Senswave.Users.Infrastructure;

public class UsersContext(IOptions<PostgreSqlOptions> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options.GetPostgresOptions<UsersContext>()), IDataProtectionKeyContext
{
    public const string SchemaName = "Users";

    public DbSet<PrivacyPolicy> PrivacyPolicies { get; set; }

    public DbSet<TermsAndConditions> TermsAndConditions { get; set; }

    public DbSet<UserConsents> UserConsents { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public DbSet<RemovedUser> RemovedUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.Entity<RemovedUser>(x =>
        {
            x.ToTable("RemovedUsers");
        });

        modelBuilder.Entity<PrivacyPolicy>(x =>
        {
            x.ToTable("PrivacyPolicies", x =>
            {
                x.HasCheckConstraint("chk_pp_version_format", "\"Version\" ~ '^[0-9]+\\.[0-9]+\\.[0-9]+$'");
            });

            x.HasIndex(x => x.Version)
             .IsUnique()
             .HasDatabaseName("PrivacyPolicies_Version");
        });

        modelBuilder.Entity<UserConsents>(x =>
        {
            x.HasOne(x => x.TermsAndConditions)
                .WithMany(x => x.UserConsents)
                .HasForeignKey(x => x.TermsAndConditionsId);

            x.HasOne(x => x.PrivacyPolicy)
                .WithMany(x => x.UserConsents)
                .HasForeignKey(x => x.PrivacyPolicyId);

            x.ToTable("UserConsents");

            x.HasIndex(x => new { x.UserId, x.PrivacyPolicyId, x.TermsAndConditionsId })
             .IsUnique()
             .HasDatabaseName("UserConsentTable_UserId_PrivacyPolicyId_TermsAndConditions");
        });

        modelBuilder.Entity<TermsAndConditions>(x =>
        {
            x.ToTable("TermsAndConditions", x =>
            {
                x.HasCheckConstraint("chk_tc_version_format", "\"Version\" ~ '^[0-9]+\\.[0-9]+\\.[0-9]+$'");
            });

            x.HasIndex(x => x.Version)
             .IsUnique()
             .HasDatabaseName("TermsAndConditions_Version");
        });
    }
}
