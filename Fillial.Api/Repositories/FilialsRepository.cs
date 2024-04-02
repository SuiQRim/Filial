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
		List<Filial> filials = new();

		string query = "SELECT Id, Name, Location FROM Filials";
		using var connection = new SqlConnection(_connectionString);
		using (SqlCommand command = new(query, connection))
		{
			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				filials.Add(new Filial
				{
					Id = (int)reader["Id"],
					Name = (string)reader["Name"],
					Location = reader.IsDBNull(reader.GetOrdinal("Location")) ? null : (string)reader["Location"]
				});
			}

			return filials;
		}
	}

	public async Task<Filial?> ReadByEmployeeIdAsync(int employeeId)
	{
		string query = "SELECT * FROM Filials " +
			"JOIN Employees ON Employees.FilialId = Filials.Id " +
			"WHERE Employees.Id = @EmployeeId";
		using (var connection = new SqlConnection(_connectionString))
		{
			SqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@EmployeeId", employeeId);

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			return await reader.ReadAsync() ? ParseEntity(reader) : null;
		}
	}

	public async Task<bool> ExistAsync(int id)
	{
		string query = "IF EXISTS " +
			"(SELECT 1 FROM Filials WHERE Id = @Id) " +
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

