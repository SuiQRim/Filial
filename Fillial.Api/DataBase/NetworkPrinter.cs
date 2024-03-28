using System.ComponentModel.DataAnnotations;

namespace PrinterFil.Api.DataBase
{
	public class NetworkPrinter : Printer
	{
		[StringLength(12)]
		public required string MacAddress { get; set; }
	}
}
