using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class PrintStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();
}
