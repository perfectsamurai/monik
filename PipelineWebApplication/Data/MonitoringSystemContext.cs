using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace monitoring;

public partial class MonitoringSystemContext : DbContext
{
    public MonitoringSystemContext()
    {
    }

    public MonitoringSystemContext(DbContextOptions<MonitoringSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advice> Advices { get; set; }

    public virtual DbSet<AdviceText> AdviceTexts { get; set; }

    public virtual DbSet<Dynamogram> Dynamograms { get; set; }

    public virtual DbSet<Guide> Guides { get; set; }

    public virtual DbSet<Ngdu> Ngdus { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Well> Wells { get; set; }

    public virtual DbSet<Workshop> Workshops { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=monitoring_system;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Advice>(entity =>
        {
            entity.HasKey(e => e.AdviceId).HasName("Advice_pkey");

            entity.ToTable("Advice");

            entity.HasIndex(e => e.RoleId, "fki_A");

            entity.HasIndex(e => e.AdviceTextId, "fki_FK_advice_advice_text");

            entity.HasIndex(e => e.DynamogramId, "fki_FK_advice_dynamogram");

            entity.HasOne(d => d.AdviceText).WithMany(p => p.Advices)
                .HasForeignKey(d => d.AdviceTextId)
                .HasConstraintName("FK_advice_advice_text");

            entity.HasOne(d => d.Dynamogram).WithMany(p => p.Advices)
                .HasForeignKey(d => d.DynamogramId)
                .HasConstraintName("FK_advice_dynamogram");

            entity.HasOne(d => d.Role).WithMany(p => p.Advices)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_advice_role");
        });

        modelBuilder.Entity<AdviceText>(entity =>
        {
            entity.HasKey(e => e.AdviceTextId).HasName("AdviceText_pkey");

            entity.ToTable("AdviceText");

            entity.Property(e => e.Status).HasColumnType("character varying");
        });

        modelBuilder.Entity<Dynamogram>(entity =>
        {
            entity.HasKey(e => e.DynamogramId).HasName("Dynamogram_pkey");

            entity.ToTable("Dynamogram");

            entity.HasIndex(e => e.UserId, "fki_FK_dynamogram_user");

            entity.HasIndex(e => e.WellId, "fki_FK_dynamogram_well");

            entity.Property(e => e.Date).HasColumnType("character varying");
            entity.Property(e => e.Opinion).HasColumnType("character varying");
            entity.Property(e => e.TypeDevice).HasColumnType("character varying");
            entity.Property(e => e.VarKpod).HasColumnName("VarKPod");
            entity.Property(e => e.VarKnap).HasColumnName("VarKNap");
            entity.Property(e => e.VarPmax).HasColumnName("VarPMax");
            entity.Property(e => e.VarPmin).HasColumnName("VarPMin");

            entity.HasOne(d => d.User).WithMany(p => p.Dynamograms)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_dynamogram_user");

            entity.HasOne(d => d.Well).WithMany(p => p.Dynamograms)
                .HasForeignKey(d => d.WellId)
                .HasConstraintName("FK_dynamogram_well");
        });

        modelBuilder.Entity<Guide>(entity =>
        {
            entity.HasKey(e => e.GuideId).HasName("Guide_pkey");

            entity.ToTable("Guide");

            entity.HasIndex(e => e.WellId, "fki_FK_guide_well");

            entity.Property(e => e.VarKnap).HasColumnName("VarKNap");
            entity.Property(e => e.VarKpod).HasColumnName("VarKPod");
            entity.Property(e => e.VarPmax).HasColumnName("VarPMax");
            entity.Property(e => e.VarPmin).HasColumnName("VarPMin");

            entity.HasOne(d => d.Well).WithMany(p => p.Guides)
                .HasForeignKey(d => d.WellId)
                .HasConstraintName("FK_guide_well");
        });

        modelBuilder.Entity<Ngdu>(entity =>
        {
            entity.HasKey(e => e.NgduId).HasName("Ngdu_pkey");

            entity.ToTable("Ngdu");

            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("Role_pkey");

            entity.ToTable("Role");

            entity.Property(e => e.Name).HasColumnType("character varying");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("User_pkey");

            entity.ToTable("User");

            entity.HasIndex(e => e.RoleId, "fki_1");

            entity.HasIndex(e => e.NgduId, "fki_FK_user_ngdu");

            entity.Property(e => e.Email).HasColumnType("character varying");
            entity.Property(e => e.FirstName).HasColumnType("character varying");
            entity.Property(e => e.LastName).HasColumnType("character varying");
            entity.Property(e => e.MiddleName).HasColumnType("character varying");
            entity.Property(e => e.Password).HasColumnType("character varying");
            entity.Property(e => e.Phone).HasColumnType("character varying");

            entity.HasOne(d => d.Ngdu).WithMany(p => p.Users)
                .HasForeignKey(d => d.NgduId)
                .HasConstraintName("FK_user_ngdu");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_user_role");
        });

        modelBuilder.Entity<Well>(entity =>
        {
            entity.HasKey(e => e.WellId).HasName("Well_pkey");

            entity.ToTable("Well");

            entity.HasIndex(e => e.WorkshopId, "fki_FK_well_workshop");

            entity.Property(e => e.Name).HasColumnType("character varying");
            entity.Property(e => e.TypeNp).HasColumnType("character varying");

            entity.HasOne(d => d.Workshop).WithMany(p => p.Wells)
                .HasForeignKey(d => d.WorkshopId)
                .HasConstraintName("FK_well_workshop");
        });

        modelBuilder.Entity<Workshop>(entity =>
        {
            entity.HasKey(e => e.WorkshopId).HasName("Workshop_pkey");

            entity.ToTable("Workshop");

            entity.HasIndex(e => e.NgduId, "fki_FK_workshop_ngdu");

            entity.Property(e => e.Name).HasColumnType("character varying");

            entity.HasOne(d => d.Ngdu).WithMany(p => p.Workshops)
                .HasForeignKey(d => d.NgduId)
                .HasConstraintName("FK_workshop_ngdu");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
