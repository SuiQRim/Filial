using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Factories;

public class LocalPrinterCreator : PrinterCreator
{
	public override Printer PrinterFactory(int id, string name, string macAddress)
	{
		return new Printer
		{
			Id = id,
			Name = name
		};
	}
}