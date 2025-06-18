using System;
using System.Collections.Generic;

namespace apiPB.Models;

public partial class VwApiWorker
{
    public int WorkerId { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Pin { get; set; }

    public string? Password { get; set; }

    public string? TipoUtente { get; set; }

    public string StorageVersamenti { get; set; } = null!;

    public string Storage { get; set; } = null!;

    public string LastLogin { get; set; } = null!;
}
