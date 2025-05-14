using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class A3AppSetting
{
    public int SettingsId { get; set; }

    public string? MagoUrl { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Company { get; set; }

    public int? SpecificatorType { get; set; }

    public bool? Closed { get; set; }
}
