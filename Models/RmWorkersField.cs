using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class RmWorkersField
{
    public int WorkerId { get; set; }

    public short Line { get; set; }

    public string? FieldName { get; set; }

    public string? FieldValue { get; set; }

    public string? Notes { get; set; }

    public string? HideOnLayout { get; set; }

    public DateTime Tbcreated { get; set; }

    public DateTime Tbmodified { get; set; }

    public int TbcreatedId { get; set; }

    public int TbmodifiedId { get; set; }

    public virtual RmWorker Worker { get; set; } = null!;
}
