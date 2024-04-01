using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Factories;

public class NetworkPrinterCreator : PrinterCreator
{
	public override Printer PrinterFactory(int id, string name, string macAddress)
	{
		return new NetworkPrinter
		{
			Id = id,
			Name = name,
			MacAddress = macAddress
		};
	}
}