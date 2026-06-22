using Microsoft.EntityFrameworkCore;
using Senswave.Automations.Domain.Entities;
using Senswave.Infrastructure.Extensions;
using Senswave.Infrastructure.Persistence;
using System.Text.Json.Nodes;

namespace Senswave.Automations.Infrastructure;

public class AutomationsContext(IOptions<PostgreSqlOptions> options) : DbContext(options.GetPostgresOptions<AutomationsContext>())
{
    public DbSet<Automation> Automations { get; set; }
    public DbSet<AutomationCondition> AutomationConditions { get; set; }
    public DbSet<AutomationResult> AutomationResults { get; set; }
    public DbSet<HomeReference> HomeReferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Automations");

        modelBuilder.Entity<AutomationCondition>(entity =>
        {
            entity.Property(c => c.ConditionConfiguration)
                .HasConversion(v => v.ToJsonString(null),
                    v => JsonObject.Parse(v, null, default)!.AsObject());
        });


        modelBuilder.Entity<AutomationResult>(entity =>
        {
            entity.Property(r => r.ValueToSend)
                .HasConversion(v => v.ToJsonString(null),
                    v => JsonNode.Parse(v, null, default)!.AsValue());
        });

        modelBuilder.Entity<Automation>(entity =>
        {
            entity.ToTable("Automations");

            entity.HasIndex(v => new { v.Name, v.HomeReferenceId })
                .IsUnique()
                .HasDatabaseName("UX_Home_AutomationName");

            entity.HasMany(a => a.Results)
                .WithOne(r => r.Automation)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(a => a.Conditions)
                .WithOne(c => c.Automation)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.HomesReference)
                .WithMany(hr => hr.Automations)
                .HasForeignKey(a => a.HomeReferenceId);
        });

        modelBuilder.Entity<HomeReference>(entity =>
        {
            entity.ToTable("HomeReferences")
                .HasIndex(hr => new { hr.OwnerId, hr.HomeId })
                .IsUnique()
                .HasDatabaseName("UX_HomeReference_OwnerId_HomeId");
        });

    }
}
