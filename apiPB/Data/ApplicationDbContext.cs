using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using apiPB.Models;

namespace apiPB.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<A3AppInventario> A3AppInventarios { get; set; }

    public virtual DbSet<A3AppPrelMat> A3AppPrelMats { get; set; }

    public virtual DbSet<A3AppRegOre> A3AppRegOres { get; set; }

    public virtual DbSet<A3AppSetting> A3AppSettings { get; set; }

    public virtual DbSet<VwApiGiacenze> VwApiGiacenzes { get; set; }

    public virtual DbSet<VwApiJob> VwApiJobs { get; set; }

    public virtual DbSet<VwApiMostep> VwApiMosteps { get; set; }

    public virtual DbSet<VwApiMostepsMocomponent> VwApiMostepsMocomponents { get; set; }

    public virtual DbSet<VwApiWorker> VwApiWorkers { get; set; }

    public virtual DbSet<VwApiWorkersfield> VwApiWorkersfields { get; set; }

    public virtual DbSet<VwOmActionMessage> VwOmActionMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:LocalA3Db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<A3AppInventario>(entity =>
        {
            entity.HasKey(e => e.InvId).HasName("PK__A3_app_i__9DC82C6AFA723957");

            entity.ToTable("A3_app_inventario");

            entity.Property(e => e.BarCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.DataImp).HasColumnType("datetime");
            entity.Property(e => e.Description).IsUnicode(false);
            entity.Property(e => e.Item).IsUnicode(false);
            entity.Property(e => e.SavedDate).HasColumnType("datetime");
            entity.Property(e => e.Storage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserImp)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<A3AppPrelMat>(entity =>
        {
            entity.HasKey(e => e.PrelMatId).HasName("PK__A3_app_p__60B8A8DC9287EC21");

            entity.ToTable("A3_app_prel_mat");

            entity.Property(e => e.Alternate)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.BarCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Bom)
                .IsUnicode(false)
                .HasColumnName("BOM");
            entity.Property(e => e.Component).IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.DataImp).HasColumnType("datetime");
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Moid).HasColumnName("MOId");
            entity.Property(e => e.Mono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MONo");
            entity.Property(e => e.OperDesc).IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SavedDate).HasColumnType("datetime");
            entity.Property(e => e.Storage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UoM)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserImp)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WC");
        });

        modelBuilder.Entity<A3AppRegOre>(entity =>
        {
            entity.HasKey(e => e.RegOreId).HasName("PK__A3_app_r__8CC8D9345A637FD5");

            entity.ToTable("A3_app_reg_ore");

            entity.Property(e => e.Alternate)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Bom)
                .IsUnicode(false)
                .HasColumnName("BOM");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.DataImp).HasColumnType("datetime");
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Moid).HasColumnName("MOId");
            entity.Property(e => e.Mono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MONo");
            entity.Property(e => e.OperDesc).IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.SavedDate).HasColumnType("datetime");
            entity.Property(e => e.Storage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Uom)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("uom");
            entity.Property(e => e.UserImp)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WC");
        });

        modelBuilder.Entity<A3AppSetting>(entity =>
        {
            entity.HasKey(e => e.SettingsId);

            entity.ToTable("A3_app_Settings");

            entity.Property(e => e.SettingsId).HasDefaultValue(1);
            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MagoUrl).IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RectificationReasonNegative)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RectificationReasonPositive)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Storage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwApiGiacenze>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_giacenze");

            entity.Property(e => e.BarCode)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Description)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Item)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Storage)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.UoM)
                .HasMaxLength(8)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VwApiJob>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_job");

            entity.Property(e => e.Description)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("job");
        });

        modelBuilder.Entity<VwApiMostep>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_mosteps");

            entity.Property(e => e.Alternate)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Bom)
                .HasMaxLength(21)
                .IsUnicode(false)
                .HasColumnName("BOM");
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Moid).HasColumnName("MOId");
            entity.Property(e => e.Mono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MONo");
            entity.Property(e => e.OperDesc)
                .HasMaxLength(96)
                .IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Storage)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.Uom)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("uom");
            entity.Property(e => e.Variant)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("WC");
        });

        modelBuilder.Entity<VwApiMostepsMocomponent>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_mosteps_mocomponents");

            entity.Property(e => e.Alternate)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.BarCode)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Bom)
                .HasMaxLength(21)
                .IsUnicode(false)
                .HasColumnName("BOM");
            entity.Property(e => e.Component)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.ItemDesc)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Moid).HasColumnName("MOId");
            entity.Property(e => e.Mono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MONo");
            entity.Property(e => e.OperDesc)
                .HasMaxLength(96)
                .IsUnicode(false);
            entity.Property(e => e.Operation)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.PrelUoM)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Storage)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.UoM)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("WC");
        });

        modelBuilder.Entity<VwApiWorker>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_workers");

            entity.Property(e => e.LastLogin)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("PIN");
            entity.Property(e => e.Storage)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.StorageVersamenti)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.TipoUtente)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.WorkerId).HasColumnName("WorkerID");
        });

        modelBuilder.Entity<VwApiWorkersfield>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_api_workersfield");

            entity.Property(e => e.FieldName)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.FieldValue)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.HideOnLayout)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Notes)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Tbcreated)
                .HasColumnType("datetime")
                .HasColumnName("TBCreated");
            entity.Property(e => e.TbcreatedId).HasColumnName("TBCreatedID");
            entity.Property(e => e.Tbmodified)
                .HasColumnType("datetime")
                .HasColumnName("TBModified");
            entity.Property(e => e.TbmodifiedId).HasColumnName("TBModifiedID");
            entity.Property(e => e.WorkerId).HasColumnName("WorkerID");
        });

        modelBuilder.Entity<VwOmActionMessage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_om_action_messages");

            entity.Property(e => e.ActionMessage)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Alternate)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Bom)
                .HasMaxLength(21)
                .IsUnicode(false)
                .HasColumnName("BOM");
            entity.Property(e => e.Closed)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ConfirmChildMos)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("ConfirmChildMOs");
            entity.Property(e => e.DeliveryDate).HasColumnType("datetime");
            entity.Property(e => e.Job)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.MessageText)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.Moid).HasColumnName("MOId");
            entity.Property(e => e.Mono)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MONo");
            entity.Property(e => e.Mostatus).HasColumnName("MOStatus");
            entity.Property(e => e.Operation)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.PickMaterialQtyGreater)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ProductionLotNumber)
                .HasMaxLength(16)
                .IsUnicode(false);
            entity.Property(e => e.ReturnMaterialQtyLower)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Specificator)
                .HasMaxLength(12)
                .IsUnicode(false);
            entity.Property(e => e.Storage)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(21)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("WC");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
