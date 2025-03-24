using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class RmWorker
{
    public int WorkerId { get; set; }

    public string? Password { get; set; }

    public string? PasswordMustBeChanged { get; set; }

    public string? PasswordCannotChange { get; set; }

    public string? PasswordNeverExpire { get; set; }

    public string? PasswordNotRenewable { get; set; }

    public DateTime? PasswordExpirationDate { get; set; }

    public short? PasswordAttemptsNumber { get; set; }

    public string? Title { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public int? Gender { get; set; }

    public string? CompanyLogin { get; set; }

    public string? DomicilyAddress { get; set; }

    public string? DomicilyCity { get; set; }

    public string? DomicilyCounty { get; set; }

    public string? DomicilyZip { get; set; }

    public string? DomicilyCountry { get; set; }

    public string? DomicilyFc { get; set; }

    public string? DomicilyIsocode { get; set; }

    public string? Telephone1 { get; set; }

    public string? Telephone2 { get; set; }

    public string? Telephone3 { get; set; }

    public string? Telephone4 { get; set; }

    public string? Email { get; set; }

    public string? Url { get; set; }

    public string? SkypeId { get; set; }

    public string? CostCenter { get; set; }

    public double? HourlyCost { get; set; }

    public string? Notes { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? CityOfBirth { get; set; }

    public string? CivilStatus { get; set; }

    public string? RegisterNumber { get; set; }

    public DateTime? EmploymentDate { get; set; }

    public DateTime? ResignationDate { get; set; }

    public string? ImagePath { get; set; }

    public string? HideOnLayout { get; set; }

    public string? Disabled { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Pin { get; set; }

    public string? Branch { get; set; }

    public string? Address2 { get; set; }

    public string? StreetNo { get; set; }

    public string? District { get; set; }

    public string? FederalState { get; set; }

    public string? IsRsenabled { get; set; }

    public string? Cgmmanager { get; set; }

    public DateTime Tbcreated { get; set; }

    public DateTime Tbmodified { get; set; }

    public int TbcreatedId { get; set; }

    public int TbmodifiedId { get; set; }

    public Guid Tbguid { get; set; }

    public string? CgmviewAllData { get; set; }

    public string? OmallowSetQuantity { get; set; }

    public int? OmprocessingType { get; set; }

    public int? OmresourceDefault { get; set; }

    public string? SenderEmail { get; set; }

    public string? Ccemail { get; set; }

    public string? Bccemail { get; set; }

    public double? SpendingLimit { get; set; }

    public virtual ICollection<RmWorkersField> RmWorkersFields { get; set; } = new List<RmWorkersField>();
}
