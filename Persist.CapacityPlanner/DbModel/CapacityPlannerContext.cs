using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Persist.CapacityPlanner.DbModel.Entities;

namespace Persist.CapacityPlanner.DbModel;

public partial class CapacityPlannerContext : DbContext
{
    public CapacityPlannerContext(DbContextOptions<CapacityPlannerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actual> Actual { get; set; }

    public virtual DbSet<Allocation> Allocation { get; set; }

    public virtual DbSet<Capacity> Capacity { get; set; }

    public virtual DbSet<Demand> Demand { get; set; }

    public virtual DbSet<DemandSkill> DemandSkill { get; set; }

    public virtual DbSet<ForecastResult> ForecastResult { get; set; }

    public virtual DbSet<Implementation> Implementation { get; set; }

    public virtual DbSet<ImplementationTemplate> ImplementationTemplate { get; set; }

    public virtual DbSet<ImplementationTemplatePhase> ImplementationTemplatePhase { get; set; }

    public virtual DbSet<ImplementationTemplatePhaseDemand> ImplementationTemplatePhaseDemand { get; set; }

    public virtual DbSet<ImplementationTemplatePhaseDemandSkill> ImplementationTemplatePhaseDemandSkill { get; set; }

    public virtual DbSet<LookupAllocationType> LookupAllocationType { get; set; }

    public virtual DbSet<LookupEmploymentType> LookupEmploymentType { get; set; }

    public virtual DbSet<LookupImplementationCategory> LookupImplementationCategory { get; set; }

    public virtual DbSet<LookupStatus> LookupStatus { get; set; }

    public virtual DbSet<LookupWorkstreamCategory> LookupWorkstreamCategory { get; set; }

    public virtual DbSet<Module> Module { get; set; }

    public virtual DbSet<Person> Person { get; set; }

    public virtual DbSet<PersonSkill> PersonSkill { get; set; }

    public virtual DbSet<Platform> Platform { get; set; }

    public virtual DbSet<Role> Role { get; set; }

    public virtual DbSet<Scenario> Scenario { get; set; }

    public virtual DbSet<Skill> Skill { get; set; }

    public virtual DbSet<Workstream> Workstream { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actual>(entity =>
        {
            entity.Property(e => e.ActualId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ActualID");
            entity.Property(e => e.ActualHours).HasColumnType("decimal(9, 2)");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.WorkstreamId).HasColumnName("WorkstreamID");

            entity.HasOne(d => d.Person).WithMany(p => p.Actual)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actual_Person");

            entity.HasOne(d => d.Workstream).WithMany(p => p.Actual)
                .HasForeignKey(d => d.WorkstreamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Actual_Workstream");
        });

        modelBuilder.Entity<Allocation>(entity =>
        {
            entity.HasKey(e => e.AllocationId).HasName("PK_Allocations");

            entity.Property(e => e.AllocationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AllocationID");
            entity.Property(e => e.AllocationPct).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.AllocationType).HasMaxLength(50);
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.Source).HasMaxLength(100);
            entity.Property(e => e.WorkstreamId).HasColumnName("WorkstreamID");

            entity.HasOne(d => d.Person).WithMany(p => p.Allocation)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Allocation_Person");

            entity.HasOne(d => d.Workstream).WithMany(p => p.Allocation)
                .HasForeignKey(d => d.WorkstreamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Allocation_Workstream");
        });

        modelBuilder.Entity<Capacity>(entity =>
        {
            entity.Property(e => e.CapacityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("CapacityID");
            entity.Property(e => e.AvailableFte)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("AvailableFTE");
            entity.Property(e => e.EffectiveUtilization).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Capacity)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Capacity_Role");
        });

        modelBuilder.Entity<Demand>(entity =>
        {
            entity.Property(e => e.DemandId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("DemandID");
            entity.Property(e => e.Confidence).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.RequiredFte)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("RequiredFTE");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.WorkstreamId).HasColumnName("WorkstreamID");

            entity.HasOne(d => d.Role).WithMany(p => p.Demand)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Demand_Role");

            entity.HasOne(d => d.Workstream).WithMany(p => p.Demand)
                .HasForeignKey(d => d.WorkstreamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Demand_Workstream");
        });

        modelBuilder.Entity<DemandSkill>(entity =>
        {
            entity.HasIndex(e => new { e.DemandId, e.SkillId }, "UQ_DemandSkill").IsUnique();

            entity.Property(e => e.DemandSkillId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("DemandSkillID");
            entity.Property(e => e.DemandId).HasColumnName("DemandID");
            entity.Property(e => e.PriorityLevel).HasMaxLength(50);
            entity.Property(e => e.SkillId).HasColumnName("SkillID");

            entity.HasOne(d => d.Demand).WithMany(p => p.DemandSkill)
                .HasForeignKey(d => d.DemandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DemandSkill_Demand");

            entity.HasOne(d => d.Skill).WithMany(p => p.DemandSkill)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DemandSkill_Skill");
        });

        modelBuilder.Entity<ForecastResult>(entity =>
        {
            entity.HasKey(e => e.ForecastId);

            entity.Property(e => e.ForecastId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ForecastID");
            entity.Property(e => e.AvailableFte)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("AvailableFTE");
            entity.Property(e => e.LastComputed).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.RequiredFte)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("RequiredFTE");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.ScenarioId).HasColumnName("ScenarioID");
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.UtilizationPct)
                .HasComputedColumnSql("(case when [AvailableFTE]=(0) then (0) else [RequiredFTE]/[AvailableFTE] end)", true)
                .HasColumnType("decimal(21, 12)");
            entity.Property(e => e.VarianceFte)
                .HasComputedColumnSql("([AvailableFTE]-[RequiredFTE])", true)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("VarianceFTE");

            entity.HasOne(d => d.Role).WithMany(p => p.ForecastResult)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForecastResult_Roles");

            entity.HasOne(d => d.Scenario).WithMany(p => p.ForecastResult)
                .HasForeignKey(d => d.ScenarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ForecastResult_Scenarios");
        });

        modelBuilder.Entity<Implementation>(entity =>
        {
            entity.HasIndex(e => new { e.PlatformId, e.ClientName }, "UQ_Implementation_Client").IsUnique();

            entity.Property(e => e.ImplementationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ImplementationID");
            entity.Property(e => e.ClientName).HasMaxLength(200);
            entity.Property(e => e.ModuleId).HasColumnName("ModuleID");
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Module).WithMany(p => p.Implementation)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_Implementation_Module");

            entity.HasOne(d => d.Platform).WithMany(p => p.Implementation)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Implementation_Platform");
        });

        modelBuilder.Entity<ImplementationTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateId);

            entity.HasIndex(e => e.Name, "UQ_ImplementationTemplate_Name").IsUnique();

            entity.Property(e => e.TemplateId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("TemplateID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Name).HasMaxLength(150);
        });

        modelBuilder.Entity<ImplementationTemplatePhase>(entity =>
        {
            entity.HasKey(e => e.PhaseId);

            entity.Property(e => e.PhaseId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PhaseID");
            entity.Property(e => e.DefaultStatus).HasMaxLength(50);
            entity.Property(e => e.PhaseName).HasMaxLength(100);
            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

            entity.HasOne(d => d.Template).WithMany(p => p.ImplementationTemplatePhase)
                .HasForeignKey(d => d.TemplateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImplementationTemplatePhase_Template");
        });

        modelBuilder.Entity<ImplementationTemplatePhaseDemand>(entity =>
        {
            entity.HasKey(e => e.PhaseDemandId);

            entity.HasIndex(e => new { e.PhaseId, e.RoleId }, "UQ_ImplementationTemplatePhaseDemand").IsUnique();

            entity.Property(e => e.PhaseDemandId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PhaseDemandID");
            entity.Property(e => e.DefaultFte)
                .HasColumnType("decimal(9, 2)")
                .HasColumnName("DefaultFTE");
            entity.Property(e => e.PhaseId).HasColumnName("PhaseID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Phase).WithMany(p => p.ImplementationTemplatePhaseDemand)
                .HasForeignKey(d => d.PhaseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImplementationTemplatePhaseDemand_Phase");

            entity.HasOne(d => d.Role).WithMany(p => p.ImplementationTemplatePhaseDemand)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImplementationTemplatePhaseDemand_Role");
        });

        modelBuilder.Entity<ImplementationTemplatePhaseDemandSkill>(entity =>
        {
            entity.HasKey(e => e.PhaseDemandSkillId);

            entity.HasIndex(e => new { e.PhaseDemandId, e.SkillId }, "UQ_ImplementationTemplatePhaseDemandSkill").IsUnique();

            entity.Property(e => e.PhaseDemandSkillId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PhaseDemandSkillID");
            entity.Property(e => e.PhaseDemandId).HasColumnName("PhaseDemandID");
            entity.Property(e => e.PriorityLevel).HasMaxLength(50);
            entity.Property(e => e.SkillId).HasColumnName("SkillID");

            entity.HasOne(d => d.PhaseDemand).WithMany(p => p.ImplementationTemplatePhaseDemandSkill)
                .HasForeignKey(d => d.PhaseDemandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImplementationTemplatePhaseDemandSkill_PhaseDemand");

            entity.HasOne(d => d.Skill).WithMany(p => p.ImplementationTemplatePhaseDemandSkill)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ImplementationTemplatePhaseDemandSkill_Skill");
        });

        modelBuilder.Entity<LookupAllocationType>(entity =>
        {
            entity.HasKey(e => e.AllocationType).HasName("PK__Lookup_A__E6A773177BDEC122");

            entity.ToTable("Lookup_AllocationType");

            entity.Property(e => e.AllocationType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<LookupEmploymentType>(entity =>
        {
            entity.HasKey(e => e.EmploymentType).HasName("PK__Lookup_E__9B151AAF1E182C52");

            entity.ToTable("Lookup_EmploymentType");

            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<LookupImplementationCategory>(entity =>
        {
            entity.HasKey(e => e.ImplementationCategory).HasName("PK__Lookup_I__4391D14B753A8D34");

            entity.ToTable("Lookup_ImplementationCategory");

            entity.Property(e => e.ImplementationCategory).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<LookupStatus>(entity =>
        {
            entity.HasKey(e => e.Status).HasName("PK__Lookup_S__3A15923E44402035");

            entity.ToTable("Lookup_Status");

            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<LookupWorkstreamCategory>(entity =>
        {
            entity.HasKey(e => e.WorkstreamCategory).HasName("PK__Lookup_W__03336CD4EFF44E8F");

            entity.ToTable("Lookup_WorkstreamCategory");

            entity.Property(e => e.WorkstreamCategory).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasIndex(e => new { e.PlatformId, e.Name }, "UQ_Module_Platform_Name").IsUnique();

            entity.Property(e => e.ModuleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ModuleID");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.PlatformId).HasColumnName("PlatformID");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Platform).WithMany(p => p.Module)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Module_Platform");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.Property(e => e.PersonId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PersonID");
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<PersonSkill>(entity =>
        {
            entity.HasIndex(e => new { e.PersonId, e.SkillId }, "UQ_PersonSkill").IsUnique();

            entity.Property(e => e.PersonSkillId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PersonSkillID");
            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.ProficiencyLevel).HasMaxLength(50);
            entity.Property(e => e.SkillId).HasColumnName("SkillID");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonSkill)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonSkill_Person");

            entity.HasOne(d => d.Skill).WithMany(p => p.PersonSkill)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonSkill_Skill");
        });

        modelBuilder.Entity<Platform>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Platform_Name").IsUnique();

            entity.Property(e => e.PlatformId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("PlatformID");
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Role_Name").IsUnique();

            entity.Property(e => e.RoleId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("RoleID");
            entity.Property(e => e.DefaultUtilizationTarget).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Scenario>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Scenario_Name").IsUnique();

            entity.Property(e => e.ScenarioId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ScenarioID");
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Name).HasMaxLength(150);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Scenario)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK_Scenario_CreatedBy");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Skill_Name").IsUnique();

            entity.Property(e => e.SkillId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("SkillID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Workstream>(entity =>
        {
            entity.HasIndex(e => e.Name, "UQ_Workstreams_Name").IsUnique();

            entity.Property(e => e.WorkstreamId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkstreamID");
            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.ImplementationId).HasColumnName("ImplementationID");
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Implementation).WithMany(p => p.Workstream)
                .HasForeignKey(d => d.ImplementationId)
                .HasConstraintName("FK_Workstream_Implementation");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
