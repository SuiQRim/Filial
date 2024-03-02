using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase.Entities;

public partial class ConnectionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PrintingDevice> PrintingDevices { get; set; } = new List<PrintingDevice>();
}
