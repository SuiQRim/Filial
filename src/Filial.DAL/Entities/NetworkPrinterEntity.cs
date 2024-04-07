namespace PFilial.DAL.Entities;

public record NetworkPrinterEntity : PrinterEntity
{
	public NetworkPrinterEntity(int id, string name, string type, string macAddress) : base (id, name, type)
	{
		MacAddress = macAddress;
	}
	public string MacAddress { get; init; }
}

