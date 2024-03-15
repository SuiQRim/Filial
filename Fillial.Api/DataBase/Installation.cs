using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class Installation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DeviceId { get; set; }

    public int FilialId { get; set; }

    public byte Order { get; set; }

    public virtual PrintingDevice Device { get; set; } = null!;

    public virtual Filial Filial { get; set; } = null!;

    public virtual ICollection<Filial> Filials { get; set; } = new List<Filial>();
}
