using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public abstract class Printer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Installation> Installations { get; set; } = new List<Installation>();
}
