using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase.Entities;

public partial class PrintJob
{
    public int Id { get; set; }

    public string Task { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int InstallationOrder { get; set; }

    public int LayerCount { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
