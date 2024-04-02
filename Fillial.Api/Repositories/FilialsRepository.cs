using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

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
		using (var connection = new SqlConnection(_connectionString))
		{
			await connection.OpenAsync();

			SqlCommand command = new (query, connection);

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
		}

		return filials;
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
}

