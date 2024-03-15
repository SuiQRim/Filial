using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;
using PrinterFil.Api.Models;
using System.Globalization;
using System.Text;

namespace PrinterFil.Api.Services
{
	public class PrintingJobImporterCSV : IPrintingJobImporter
	{
        public PrintingJobImporterCSV()
        {
			_configuration = new(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
				Delimiter = ";",
				Encoding = Encoding.UTF8
			};
		}

        public PrintingJobImporterCSV(CsvConfiguration configuration)
        {
			_configuration = configuration;
		}

		private readonly CsvConfiguration _configuration;

		public IEnumerable<PrintJobDTO> Parse(IFormFile file)
		{
			string[] lines;
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				lines = ReadFirstLines(reader, 100);
			}

			List<PrintJobDTO> printJobDTOs = new(lines.Length);

			using (var reader = new StringReader(string.Join(Environment.NewLine, lines)))
			using (var csv = new CsvReader(reader, _configuration))
			{
				while (csv.Read())
				{
					try
					{

						PrintJobDTO pj = new(
							csv.GetField<string>(0),
							csv.GetField<int>(1),
							csv.GetField<byte>(2),
							csv.GetField<int>(3)
						);

						printJobDTOs.Add(pj);
					}
					catch (CsvHelperException ex)
					when (ex is ValidationException || ex is TypeConverterException || ex is CsvHelper.MissingFieldException)
					{
						continue;
					}
				}
			}
			return printJobDTOs;
		}

		private static string[] ReadFirstLines(StreamReader reader, int maxLines)
		{
			List<string> lines = new(maxLines);
			for (int i = 0; i < maxLines; i++)
			{
				var line = reader.ReadLine();

				if (line == null)
					break;

				lines.Add(line);
			}
			return lines.ToArray();
		}
	}
}
