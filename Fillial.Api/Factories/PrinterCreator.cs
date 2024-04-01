using PrinterFil.Api.DataBase;

namespace PrinterFil.Api.Factories;

public abstract class PrinterCreator
{
	public abstract Printer PrinterFactory(int id, string name, string macAddress);
}