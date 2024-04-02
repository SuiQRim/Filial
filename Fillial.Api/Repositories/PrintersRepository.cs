using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Factories;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories;

public class PrintersRepository : IPrintersRepository
{
	private readonly string _connectionString;
	public PrintersRepository(string connection)
	{
		_connectionString = connection;
	}

	// Пока думаю что с этим сделать)
	private static readonly Dictionary<Type, PrinterCreator> Creators = new()
	{
		{typeof(Printer), new LocalPrinterCreator() },
		{typeof(NetworkPrinter), new NetworkPrinterCreator() },
	};

	private static readonly Dictionary<Type, string> Types = new()
	{
		{typeof(Printer), "Local" },
		{typeof(NetworkPrinter), "Network" },
	};

	public async Task<IEnumerable<Printer>> ReadAsync()
	{
		List<Printer> printers = new ();

		string query = "SELECT Id, Name, MacAddress, Type FROM Printers";
		using (SqlConnection connection = new (_connectionString))
		{
			SqlCommand command = new (query, connection);
			await connection.OpenAsync(); 

			using (SqlDataReader reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					int id = (int)reader["Id"];
					string name = (string)reader["Name"];
					string type = (string)reader["Type"];
					string? macAddress = reader.IsDBNull(reader.GetOrdinal("MacAddress")) ? null : (string)reader["MacAddress"];

					if (type == "Local")
					{
						printers.Add(Creators[typeof(Printer)].PrinterFactory(id, name, macAddress));
					}
					else if (type == "Network")
					{
						printers.Add(Creators[typeof(NetworkPrinter)].PrinterFactory(id, name, macAddress));
					}
				}
			}
		}
		return printers;
	}

	public async Task<IEnumerable<Printer>> ReadAsync<T>() where T : Printer
	{
		string type = Types[typeof(T)];
		string query = "SELECT Id, Name, MacAddress FROM Printers WHERE Type = @Type";
		using (SqlConnection connection = new(_connectionString))
		{
			await connection.OpenAsync();

			using SqlCommand command = new(query, connection);
			command.Parameters.Add(new SqlParameter("@Type", type));

			List<Printer> printers = new();
			using (SqlDataReader reader = await command.ExecuteReaderAsync())
			{
				while (await reader.ReadAsync())
				{
					int id = (int)reader["Id"];
					string name = (string)reader["Name"];
					string? macAddress = reader.IsDBNull(reader.GetOrdinal("MacAddress")) ? null : (string)reader["MacAddress"];

					Printer printer = Creators[typeof(T)].PrinterFactory(id, name, macAddress);
					printers.Add(printer);
				}
			}
			return printers;
		}

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
			object? result = await command.ExecuteScalarAsync();
			return result != null && (int)result > 0;
		}
	}
}

