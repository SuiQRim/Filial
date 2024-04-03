using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class PrintersRepository : IPrintersRepository
{
	private readonly string _connectionString;
	public PrintersRepository(string connection)
	{
		_connectionString = connection;
	}

	public async Task<IEnumerable<Printer>> ReadAsync()
	{
		List<Printer> printers = [];
		string query = "SELECT Id, Name, MacAddress, Type FROM Printers";
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection)) { 

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				string type = (string)reader["Type"];
				Printer? printer = type switch
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

	public async Task<IEnumerable<Printer>> ReadLocalAsync()
	{
		List<Printer> printers = [];
		string query = "SELECT Id, Name FROM Printers WHERE Type = @Type";
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection))
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

	public async Task<IEnumerable<NetworkPrinter>> ReadNetworkAsync()
	{
		List<NetworkPrinter> printers = [];
		string query = "SELECT Id, Name, MacAddress FROM Printers WHERE Type = @Type";
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection))
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
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection))
		{
			command.Parameters.AddWithValue("@Id", id);

			await connection.OpenAsync();
			return (int?)await command.ExecuteScalarAsync() == 1;
		}
	}

	private Printer ReadLocalPrinter(SqlDataReader reader)
	{
		int id = (int)reader["Id"];
		string name = (string)reader["Name"];
		return new Printer { Id = id, Name = name };
	}

	private NetworkPrinter ReadNetworkPrinter(SqlDataReader reader)
	{
		int id = (int)reader["Id"];
		string name = (string)reader["Name"];
		string macAddress = (string)reader["MacAddress"];
		return new NetworkPrinter { Id = id, Name = name, MacAddress = macAddress };
	}
}

