using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwOmmessage
{
    public int MessageId { get; set; }

    public int? MessageType { get; set; }

    public int? WorkerId { get; set; }

    public DateTime? MessageDate { get; set; }

    public string? MessageText { get; set; }

    public string? UserMessage { get; set; }

    public string? Expire { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int? Moid { get; set; }

    public short? RtgStep { get; set; }

    public string? Alternate { get; set; }

    public short? AltRtgStep { get; set; }

    public DateTime Tbcreated { get; set; }

    public DateTime Tbmodified { get; set; }

    public int TbcreatedId { get; set; }

    public int TbmodifiedId { get; set; }

    public Guid Tbguid { get; set; }
}
