using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using PFilial.BLL.Models;
using PFilial.BLL.Services.Interfaces;
using System.Globalization;
using System.Text;

namespace PFilial.BLL.Services
{
	public class PrintJobImporterCSV : IPrintJobImporter
	{
		public PrintJobImporterCSV()
		{

		}
		public PrintJobImporterCSV(CsvConfiguration configuration)
		{
			_configuration = configuration;
		}

		private readonly CsvConfiguration _configuration = new(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = false,
			Delimiter = ";",
			Encoding = Encoding.UTF8
		};

		public async Task<PrintJobDTO[]> ParseAsync(Stream stream)
		{
			const int maxLines = 100;
			string[] lines;
			using (StreamReader streamReader = new (stream))
			{
				CheckEncoding(streamReader);
				lines = await ReadFirstLinesAsync(streamReader, maxLines);
			}

			List<PrintJobDTO> printJobDTOs = new(lines.Length);

			using (StringReader stringReader = new(string.Join(Environment.NewLine, lines)))
			using (CsvReader csvReader = new(stringReader, _configuration))
			{
				while (csvReader.Read())
				{
					try
					{
						PrintJobDTO pj = new(
							csvReader.GetField<string>(0),
							csvReader.GetField<int>(1),
							csvReader.GetField<byte>(2),
							csvReader.GetField<int>(3));

						printJobDTOs.Add(pj);
					}
					catch (CsvHelperException ex) when (ex is ValidationException || ex is TypeConverterException)
					{
						continue;
					}
				}
			}

			return printJobDTOs.ToArray();
		}
		private void CheckEncoding(StreamReader streamReader)
		{
			streamReader.Peek();
			if (streamReader.CurrentEncoding != _configuration.Encoding)
				throw new InvalidOperationException("The file is not encoded in UTF-8");
		}
		private async Task<string[]> ReadFirstLinesAsync(StreamReader reader, int maxLines)
		{
			List<string> lines = new(maxLines);
			for (int i = 0; i < maxLines; i++)
			{
				string? line = await reader.ReadLineAsync();

				if (line == null)
					break;

				lines.Add(line);
			}
			return [.. lines];
		}
	}
}
