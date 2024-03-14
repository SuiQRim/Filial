using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class PrintJob
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int EmployeeId { get; set; }

    public int Order { get; set; }

    public int LayerCount { get; set; }

    public bool IsSuccessful { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
