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

    public virtual DbSet<A3AppRegOre> A3AppRegOres { get; set; }

    public virtual DbSet<MaStorage> MaStorages { get; set; }

    public virtual DbSet<RmWorker> RmWorkers { get; set; }

    public virtual DbSet<RmWorkersField> RmWorkersFields { get; set; }

    public virtual DbSet<VwApiJob> VwApiJobs { get; set; }

    public virtual DbSet<VwApiMostep> VwApiMosteps { get; set; }

    public virtual DbSet<VwApiMostepsMocomponent> VwApiMostepsMocomponents { get; set; }

    public virtual DbSet<VwApiWorker> VwApiWorkers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=SRV2022MES\\SQLEXPRESS;Initial Catalog=PIETRIBIASISRLM4;User ID=sa;Password=sa_2022;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<A3AppRegOre>(entity =>
        {
            entity.HasKey(e => e.RegOreId).HasName("PK__A3_app_r__8CC8D93438267709");

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
                .HasMaxLength(1)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Wc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("WC");
            entity.Property(e => e.WorkerId)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<MaStorage>(entity =>
        {
            entity.HasKey(e => e.Storage)
                .HasName("PK_Storages")
                .IsClustered(false);

            entity.ToTable("MA_Storages");

            entity.HasIndex(e => e.Description, "MA_Storages2");

            entity.Property(e => e.Storage)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Address)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Address2)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.AllCompanies)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.City)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CodeType).HasDefaultValue(3342336);
            entity.Property(e => e.CompanyId).HasDefaultValue(-1);
            entity.Property(e => e.ConsignmentStock)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.Country)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.County)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CrossDocking)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.CrossDockingZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Description)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Disabled)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.District)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ExcludeFromOnHand)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.ExcludeFromValuation)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.Fax)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.FederalState)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.GoodsIssueZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.GoodsReceiptZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.GroupCode)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.InspectionZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.InventoryScarcityCheckType).HasDefaultValue(25100288);
            entity.Property(e => e.InventoryShortageCheckType).HasDefaultValue(25100288);
            entity.Property(e => e.IsMainStorage)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.IsocountryCode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ISOCountryCode");
            entity.Property(e => e.LastSnapshotCertifiedDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime");
            entity.Property(e => e.ManTwoStepsPutaway)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.ManufacturingIssuePickZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ManufacturingIssueZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ManufacturingReceiptZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Notes)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.OwnedGoods)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.Priority).HasDefaultValue((short)99);
            entity.Property(e => e.ReturnZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.SalesOrdersShortageCheckType).HasDefaultValue(25100288);
            entity.Property(e => e.SalesScarcityCheckType).HasDefaultValue(25100288);
            entity.Property(e => e.SalesShortageCheckType).HasDefaultValue(25100288);
            entity.Property(e => e.ScrapZone)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SearchZoneStrReplanish)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SearchZoneStrategyPicking)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SearchZoneStrategyPutaway)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SpecTypeReplenishment).HasDefaultValue(6750211);
            entity.Property(e => e.SpecificatorReplenishment)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StockReturnStrategy).HasDefaultValue(26607618);
            entity.Property(e => e.StorageBarcodePrefix)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StorageReplenishment)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StreetNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StubBookAdjustment)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StubBookInterStorageIn)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("StubBookInterStorageIN");
            entity.Property(e => e.StubBookInterStorageOut)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("StubBookInterStorageOUT");
            entity.Property(e => e.StubBookPurchases)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StubBookSales)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.TaxJournalPurchases)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.TaxJournalSales)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Tbcreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBCreated");
            entity.Property(e => e.TbcreatedId).HasColumnName("TBCreatedID");
            entity.Property(e => e.Tbguid)
                .HasDefaultValueSql("(0x00)")
                .HasColumnName("TBGuid");
            entity.Property(e => e.Tbmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBModified");
            entity.Property(e => e.TbmodifiedId).HasColumnName("TBModifiedID");
            entity.Property(e => e.Telephone1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Telephone2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.TwoStepsPutaway)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .IsFixedLength();
            entity.Property(e => e.UsedByMrp)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("UsedByMRP");
            entity.Property(e => e.UsedByProduction)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.UsedForRetail)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.WaptransferBetweenStorages)
                .HasDefaultValue(2032336896)
                .HasColumnName("WAPTransferBetweenStorages");
            entity.Property(e => e.Wms)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("WMS");
            entity.Property(e => e.WmsactivationDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime")
                .HasColumnName("WMSActivationDate");
            entity.Property(e => e.WmsmanActivationDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime")
                .HasColumnName("WMSManActivationDate");
            entity.Property(e => e.WmsthirdParties)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("WMSThirdParties");
            entity.Property(e => e.Zipcode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("ZIPCode");
        });

        modelBuilder.Entity<RmWorker>(entity =>
        {
            entity.HasKey(e => e.WorkerId).IsClustered(false);

            entity.ToTable("RM_Workers");

            entity.HasIndex(e => new { e.CompanyLogin, e.WorkerId }, "IX_RM_Workers_1");

            entity.Property(e => e.WorkerId)
                .ValueGeneratedNever()
                .HasColumnName("WorkerID");
            entity.Property(e => e.Address2)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Bccemail)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("BCCEmail");
            entity.Property(e => e.Branch)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Ccemail)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("CCEmail");
            entity.Property(e => e.Cgmmanager)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("CGMManager");
            entity.Property(e => e.CgmviewAllData)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("CGMViewAllData");
            entity.Property(e => e.CityOfBirth)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CivilStatus)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CompanyLogin)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CostCenter)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DateOfBirth)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime");
            entity.Property(e => e.Disabled)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.District)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DomicilyAddress)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DomicilyCity)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DomicilyCountry)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DomicilyCounty)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.DomicilyFc)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("DomicilyFC");
            entity.Property(e => e.DomicilyIsocode)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("DomicilyISOCode");
            entity.Property(e => e.DomicilyZip)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Email)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.EmploymentDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime");
            entity.Property(e => e.FederalState)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Gender).HasDefaultValue(2097152);
            entity.Property(e => e.HideOnLayout)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.HourlyCost).HasDefaultValue(0.0);
            entity.Property(e => e.ImagePath)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.IsRsenabled)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength()
                .HasColumnName("IsRSEnabled");
            entity.Property(e => e.LastName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Latitude)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Longitude)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Notes)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.OmallowSetQuantity)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .IsFixedLength()
                .HasColumnName("OMAllowSetQuantity");
            entity.Property(e => e.OmprocessingType)
                .HasDefaultValue(2051407874)
                .HasColumnName("OMProcessingType");
            entity.Property(e => e.OmresourceDefault)
                .HasDefaultValue(2051473414)
                .HasColumnName("OMResourceDefault");
            entity.Property(e => e.Password)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.PasswordAttemptsNumber).HasDefaultValue((short)0);
            entity.Property(e => e.PasswordCannotChange)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.PasswordExpirationDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordMustBeChanged)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.PasswordNeverExpire)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("1")
                .IsFixedLength();
            entity.Property(e => e.PasswordNotRenewable)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("0")
                .IsFixedLength();
            entity.Property(e => e.Pin)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("PIN");
            entity.Property(e => e.RegisterNumber)
                .HasMaxLength(16)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ResignationDate)
                .HasDefaultValueSql("('17991231')")
                .HasColumnType("datetime");
            entity.Property(e => e.SenderEmail)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SkypeId)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("SkypeID");
            entity.Property(e => e.SpendingLimit).HasDefaultValueSql("('')");
            entity.Property(e => e.StreetNo)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Tbcreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBCreated");
            entity.Property(e => e.TbcreatedId).HasColumnName("TBCreatedID");
            entity.Property(e => e.Tbguid)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("TBGuid");
            entity.Property(e => e.Tbmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBModified");
            entity.Property(e => e.TbmodifiedId).HasColumnName("TBModifiedID");
            entity.Property(e => e.Telephone1)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Telephone2)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Telephone3)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Telephone4)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Title)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Url)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("URL");
        });

        modelBuilder.Entity<RmWorkersField>(entity =>
        {
            entity.HasKey(e => new { e.WorkerId, e.Line }).IsClustered(false);

            entity.ToTable("RM_WorkersFields");

            entity.Property(e => e.WorkerId).HasColumnName("WorkerID");
            entity.Property(e => e.FieldName)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.FieldValue)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.HideOnLayout)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValue("")
                .IsFixedLength();
            entity.Property(e => e.Notes)
                .HasMaxLength(64)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Tbcreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBCreated");
            entity.Property(e => e.TbcreatedId).HasColumnName("TBCreatedID");
            entity.Property(e => e.Tbmodified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("TBModified");
            entity.Property(e => e.TbmodifiedId).HasColumnName("TBModifiedID");

            entity.HasOne(d => d.Worker).WithMany(p => p.RmWorkersFields)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RM_WorkersFields_00");
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
            entity.Property(e => e.Storage)
                .HasMaxLength(4)
                .IsUnicode(false);
            entity.Property(e => e.UoM)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.Variant)
                .HasMaxLength(21)
                .IsUnicode(false);
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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
