using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase.Entities;

public partial class Installation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DeviceId { get; set; }

    public int FillialId { get; set; }

    public int Order { get; set; }

    public bool IsDefault { get; set; }

    public virtual PrintingDevice Device { get; set; } = null!;

    public virtual Filial Fillial { get; set; } = null!;
}
