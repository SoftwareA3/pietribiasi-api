using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using apiPB.Models;

namespace apiPB.Data;

public partial class ApplicationDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<MaStorage> MaStorages { get; set; }

    public virtual DbSet<RmWorker> RmWorkers { get; set; }

    public virtual DbSet<RmWorkersField> RmWorkersFields { get; set; }

    public virtual DbSet<VwWorker> VwWorkers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<VwWorker>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("Vw_Workers");

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
