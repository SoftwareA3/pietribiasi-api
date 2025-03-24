using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class MaStorage
{
    public string Storage { get; set; } = null!;

    public string? Description { get; set; }

    public string? GroupCode { get; set; }

    public int? CodeType { get; set; }

    public string? OwnedGoods { get; set; }

    public string? Address { get; set; }

    public string? Zipcode { get; set; }

    public string? City { get; set; }

    public string? County { get; set; }

    public string? Country { get; set; }

    public string? Telephone1 { get; set; }

    public string? Telephone2 { get; set; }

    public string? Fax { get; set; }

    public string? Notes { get; set; }

    public string? Disabled { get; set; }

    public short? Priority { get; set; }

    public string? UsedByProduction { get; set; }

    public string? UsedByMrp { get; set; }

    public Guid? Tbguid { get; set; }

    public string? TaxJournalSales { get; set; }

    public string? TaxJournalPurchases { get; set; }

    public string? StubBookSales { get; set; }

    public string? StubBookPurchases { get; set; }

    public string? StubBookInterStorageIn { get; set; }

    public string? StubBookInterStorageOut { get; set; }

    public string? StubBookAdjustment { get; set; }

    public string? UsedForRetail { get; set; }

    public int? InventoryShortageCheckType { get; set; }

    public int? SalesShortageCheckType { get; set; }

    public int? SalesOrdersShortageCheckType { get; set; }

    public int? InventoryScarcityCheckType { get; set; }

    public int? SalesScarcityCheckType { get; set; }

    public DateTime Tbcreated { get; set; }

    public DateTime Tbmodified { get; set; }

    public string? Wms { get; set; }

    public string? GoodsReceiptZone { get; set; }

    public string? GoodsIssueZone { get; set; }

    public string? TwoStepsPutaway { get; set; }

    public string? ReturnZone { get; set; }

    public string? ScrapZone { get; set; }

    public string? InspectionZone { get; set; }

    public string? SearchZoneStrategyPutaway { get; set; }

    public string? SearchZoneStrategyPicking { get; set; }

    public int? StockReturnStrategy { get; set; }

    public string? ManTwoStepsPutaway { get; set; }

    public string? ManufacturingReceiptZone { get; set; }

    public string? ManufacturingIssueZone { get; set; }

    public DateTime? WmsactivationDate { get; set; }

    public DateTime? WmsmanActivationDate { get; set; }

    public string? StorageBarcodePrefix { get; set; }

    public DateTime? LastSnapshotCertifiedDate { get; set; }

    public string? CrossDocking { get; set; }

    public string? CrossDockingZone { get; set; }

    public int TbcreatedId { get; set; }

    public int TbmodifiedId { get; set; }

    public string? StorageReplenishment { get; set; }

    public int? SpecTypeReplenishment { get; set; }

    public string? SpecificatorReplenishment { get; set; }

    public string? ConsignmentStock { get; set; }

    public string? IsocountryCode { get; set; }

    public string? Address2 { get; set; }

    public string? StreetNo { get; set; }

    public string? District { get; set; }

    public string? FederalState { get; set; }

    public string? ManufacturingIssuePickZone { get; set; }

    public string? ExcludeFromValuation { get; set; }

    public string? IsMainStorage { get; set; }

    public string? ExcludeFromOnHand { get; set; }

    public int? WaptransferBetweenStorages { get; set; }

    public string? SearchZoneStrReplanish { get; set; }

    public string? WmsthirdParties { get; set; }

    public string? AllCompanies { get; set; }

    public int? CompanyId { get; set; }
}
