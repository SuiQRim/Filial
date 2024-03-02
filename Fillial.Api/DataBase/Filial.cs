using System;
using System.Collections.Generic;

namespace PrinterFil.Api.DataBase;

public partial class Filial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public int DefaultInstallationId { get; set; }

    public virtual Installation DefaultInstallation { get; set; } = null!;

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Installation> Installations { get; set; } = new List<Installation>();
}
