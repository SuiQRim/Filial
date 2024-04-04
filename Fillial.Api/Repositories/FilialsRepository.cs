using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;
using System.Data.Common;

namespace PrinterFil.Api.Repositories;

public class FilialsRepository : IFilialsRepository
{
	private readonly string _connectionString;

	public FilialsRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	public async Task<IEnumerable<Filial>> ReadAsync()
	{
		List<Filial> filials = [];
		string query = "SELECT Id, Name, Location FROM Filials";

		await using SqlConnection connection = new (_connectionString);
		await using SqlCommand command = new(query, connection);

		await connection.OpenAsync();
		await using SqlDataReader reader = await command.ExecuteReaderAsync();
		while (await reader.ReadAsync())
		{
			int locationIndex = reader.GetOrdinal("Location");
			filials.Add(new Filial
			{
				Id = (int)reader["Id"],
				Name = (string)reader["Name"],
				Location = reader.IsDBNull(locationIndex) ? null : (string)reader[locationIndex]
			});
		}

		return filials;
	}

	public async Task<Filial?> ReadByEmployeeIdAsync(int employeeId)
	{
		string query = "SELECT * FROM Filials " +
			"JOIN Employees ON Employees.FilialId = Filials.Id " +
			"WHERE Employees.Id = @EmployeeId";

		await using var connection = new SqlConnection(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@EmployeeId", employeeId);

		await connection.OpenAsync();
		using SqlDataReader reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public async Task<bool> ExistAsync(int id)
	{
		string query = "IF EXISTS " +
			"(SELECT 1 FROM Filials WHERE Id = @Id) " +
			"SELECT 1 ELSE SELECT 0";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@Id", id);

		await connection.OpenAsync();
		return (int?)await command.ExecuteScalarAsync() == 1;
	}

	private static Filial ParseEntity(DbDataReader reader)
	{
		return new Filial
		{
			Id = (int)reader["Id"],
			Name = (string)reader["Name"],
			Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : (string)reader["Location"]
		};
	}
}

