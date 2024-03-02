namespace PrinterFil.Api.DataBase;

public partial class PrintingDevice
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ConnectionTypeId { get; set; }

    public string? MacAddress { get; set; }

    public virtual ConnectionType ConnectionType { get; set; } = null!;

    public virtual ICollection<Installation> Installations { get; set; } = new List<Installation>();
}
