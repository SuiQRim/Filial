using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class PrintJob
{
    public int Id { get; set; }

    public string Task { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int InstallationId { get; set; }

    public int LayerCount { get; set; }

    public int StatusId { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Installation Installation { get; set; } = null!;

    public virtual PrintStatus Status { get; set; } = null!;
}
