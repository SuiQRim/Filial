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
            
        }
        public PrintingJobImporterCSV(CsvConfiguration configuration)
        {
			_configuration = configuration;
		}

		private readonly CsvConfiguration _configuration = new (CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = false,
			Delimiter = ";",
			Encoding = Encoding.UTF8
		};

		/// <summary>
		/// Парсит csv-файл
		/// </summary>
		/// <param name="file">Файл</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Выбрасывается при несоблюдении требований к файлу</exception>
		public async Task<IEnumerable<PrintJobDTO>> ParseAsync(IFormFile file)
		{
			const int maxLines = 100;
			string[] lines;
			using (StreamReader streamReader = new (file.OpenReadStream()))
			{
				CheckEncoding(streamReader);
				lines = await ReadFirstLinesAsync(streamReader, maxLines);
			}

			List<PrintJobDTO> printJobDTOs = new (lines.Length);

			using (StringReader stringReader = new (string.Join(Environment.NewLine, lines)))
			using (CsvReader csvReader = new (stringReader, _configuration))
			{
				while (csvReader.Read())
				{
					try
					{
						PrintJobDTO pj = new (
							csvReader.GetField<string>(0),
							csvReader.GetField<int>(1),
							csvReader.GetField<byte>(2),
							csvReader.GetField<int>(3)
						);

						printJobDTOs.Add(pj);
					}
					catch (CsvHelperException ex) when (ex is ValidationException || ex is TypeConverterException)
					{
						continue;
					}
				}
			}

			return printJobDTOs;
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
