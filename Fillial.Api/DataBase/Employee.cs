using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? FilialId { get; set; }

    public virtual Filial? Filial { get; set; }

    public virtual ICollection<PrintJob> PrintJobs { get; set; } = new List<PrintJob>();
}
