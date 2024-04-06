using Microsoft.Data.SqlClient;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;

namespace PrinterFil.Api.Repositories;

public class PrintersRepository : IPrintersRepository
{
	private readonly string _connectionString;
	public PrintersRepository(string connection)
	{
		_connectionString = connection;
	}

	public async Task<IEnumerable<PrinterEntity>> ReadAsync()
	{
		List<PrinterEntity> printers = [];
		string query = "SELECT Id, Name, MacAddress, Type FROM Printers";

		await using SqlConnection connection = new(_connectionString);
		await using (SqlCommand command = new(query, connection))
		{
			await connection.OpenAsync();
			await using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				string type = (string)reader["Type"];
				PrinterEntity? printer = type switch
				{
					"Local" => ReadLocalPrinter(reader),
					"Network" => ReadNetworkPrinter(reader),
					_ => null
				};

				if (printer != null)
					printers.Add(printer);
			}
		}
		return printers;
	}

	public async Task<IEnumerable<PrinterEntity>> ReadLocalAsync()
	{
		List<PrinterEntity> printers = [];
		string query = "SELECT Id, Name FROM Printers WHERE Type = @Type";

		await using SqlConnection connection = new(_connectionString);
		await using (SqlCommand command = new(query, connection))
		{
			command.Parameters.Add(new("@Type", "Local"));

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				printers.Add(ReadLocalPrinter(reader));
			}
		}
		return printers;
	}

	public async Task<IEnumerable<NetworkPrinterEntity>> ReadNetworkAsync()
	{
		List<NetworkPrinterEntity> printers = [];
		string query = "SELECT Id, Name, MacAddress FROM Printers WHERE Type = @Type";

		await using SqlConnection connection = new(_connectionString);
		await using (SqlCommand command = new(query, connection))
		{
			command.Parameters.Add(new("@Type", "Network"));

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				printers.Add(ReadNetworkPrinter(reader));
			}
		}
		return printers;
	}

	public async Task<bool> ExistAsync(int id)
	{
		string query = "IF EXISTS " +
			"(SELECT 1 FROM Printers WHERE Id = @Id) " +
			"SELECT 1 ELSE SELECT 0";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@Id", id);

		await connection.OpenAsync();
		return (int?)await command.ExecuteScalarAsync() == 1;
	}

	private PrinterEntity ReadLocalPrinter(SqlDataReader reader)
	{
		int id = (int)reader["Id"];
		string name = (string)reader["Name"];
		return new PrinterEntity { Id = id, Name = name };
	}

	private NetworkPrinterEntity ReadNetworkPrinter(SqlDataReader reader)
	{
		int id = (int)reader["Id"];
		string name = (string)reader["Name"];
		string macAddress = (string)reader["MacAddress"];
		return new NetworkPrinterEntity { Id = id, Name = name, MacAddress = macAddress };
	}
}

