namespace PrinterFil.Api.DataBase;

public partial class ConnectionType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PrintingDevice> PrintingDevices { get; set; } = new List<PrintingDevice>();
}
