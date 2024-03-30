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
		public IEnumerable<PrintJobDTO> Parse(IFormFile file)
		{
			string[] lines;
			using (var reader = new StreamReader(file.OpenReadStream()))
			{
				reader.Peek();
				if (reader.CurrentEncoding != Encoding.UTF8)
					throw new InvalidOperationException("The file is not encoded in UTF-8");

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
					when (ex is ValidationException || ex is TypeConverterException)
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
