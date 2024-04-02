using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;
using System.Data.Common;

namespace PrinterFil.Api.Repositories;

public class InstallationsRepository : IInstallationsRepository
{
	private readonly string _connectionString;
	public InstallationsRepository(string connection)
    {
		_connectionString = connection;
	}

    public async Task<IEnumerable<Installation>> ReadAsync(int? filialId)
	{
		List<Installation> installations = new();
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = COALESCE(@FilialId, FilialId)";

		using (SqlConnection connection = new(_connectionString))
		{
			SqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@FilialId", filialId.HasValue ? filialId : DBNull.Value);

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				installations.Add(ParseEntity(reader));
			}
		}

		return installations;
	}

	public async Task<Installation?> ReadAsync(int id)
	{
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE Id = @Id";
		using (SqlConnection connection = new(_connectionString))
		{
			SqlCommand command = new(query, connection);
			command.Parameters.AddWithValue("@Id", id);

			await connection.OpenAsync();
			using SqlDataReader reader = await command.ExecuteReaderAsync();

			return await reader.ReadAsync() ? ParseEntity(reader) : null;
		}
	}

	public async Task<Installation?> ReadDefaultAsync(int filialId)
	{
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = @FilialId AND IsDefault = 1";
		using SqlConnection connection = new(_connectionString);

		SqlCommand command = new(query, connection);
		command.Parameters.AddWithValue("@FilialId", filialId);

		await connection.OpenAsync();
		using SqlDataReader reader = await command.ExecuteReaderAsync();

		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public async Task<Installation?> ReadFirstAsync(int filialId)
	{
		string query = "SELECT TOP 1 Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = @FilialId";
		using SqlConnection connection = new(_connectionString);
		
		SqlCommand command = new(query, connection);
		command.Parameters.Add(new SqlParameter("@FilialId", filialId));

		await connection.OpenAsync();
		using SqlDataReader reader = await command.ExecuteReaderAsync();

		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public async Task<int?> CreateAsync(Installation installation)
	{
		string query = "INSERT INTO Installations (Name, FilialId, DeviceId, IsDefault, [Order]) output INSERTED.ID " +
			"VALUES (@Name, @FilialId, @DeviceId, @IsDefault, @Order)";
		using (SqlConnection connection = new (_connectionString))
		using (SqlCommand command = new (query, connection))
		{
			command.Parameters.AddWithValue("@Name", installation.Name);
			command.Parameters.AddWithValue("@FilialId", installation.FilialId);
			command.Parameters.AddWithValue("@DeviceId", installation.DeviceId);
			command.Parameters.AddWithValue("@IsDefault", installation.IsDefault);
			command.Parameters.AddWithValue("@Order", installation.Order);

			await connection.OpenAsync();
			return (int?)await command.ExecuteScalarAsync();
		}
	}

	public async Task DeleteAsync(int id)
	{
		string query = "DELETE FROM Installations WHERE Id = @Id";
		using (SqlConnection connection = new(_connectionString))
		using (SqlCommand command = new(query, connection))
		{
			command.Parameters.AddWithValue("@Id", id);

			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();
		}
	}

	public async Task<bool> Exist(int? filialId = null, byte? order = null)
	{
		string query = "IF EXISTS " +
			"(SELECT 1 FROM Installations WHERE FilialId = @FilialId AND [Order] = COALESCE(@Order, [Order])) " +
			"SELECT 1 ELSE SELECT 0";
		using SqlConnection connection = new (_connectionString);
		using (SqlCommand command = new (query, connection))
		{
			command.Parameters.AddWithValue("@FilialId", 3);
			command.Parameters.AddWithValue("@Order", order.HasValue ? filialId : DBNull.Value);

			await connection.OpenAsync();
			object? result = await command.ExecuteScalarAsync();
			return result != null && (int)result > 0;
		}
	}

	public async Task<byte?> GetOrderAsync(int filialId)
	{
		// Запрос вернет минимальное не использованное число от 1 до 255
		const int maxValue = byte.MaxValue;
		string query = "SELECT MIN([Order]) + 1 FROM Installations " +
			"WHERE [Order] < @MaxValue AND [Order] + 1 NOT IN (" +
			"SELECT [Order] FROM Installations WHERE FilialId = @FilialId)";
		using SqlConnection connection = new (_connectionString);
		using (SqlCommand command = new(query, connection))
		{
			command.Parameters.AddWithValue("@MaxValue", maxValue);
			command.Parameters.AddWithValue("@FilialId", filialId);

			await connection.OpenAsync();
			object? result = await command.ExecuteScalarAsync();
			return !Convert.IsDBNull(result) ? Convert.ToByte(result) : null;
		}
		
	}

	public async Task UpdateAsync(int id, Installation installation)
	{
		string query = "UPDATE Installations SET " +
			"Name = @Name, FilialId = @FilialId, DeviceId = @DeviceId, IsDefault = @IsDefault, [Order] = @Order " +
			"WHERE Id = @Id";
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection))
		{
			command.Parameters.AddWithValue("@Id", id);
			command.Parameters.AddWithValue("@Name", installation.Name);
			command.Parameters.AddWithValue("@FilialId", installation.FilialId);
			command.Parameters.AddWithValue("@DeviceId", installation.DeviceId);
			command.Parameters.AddWithValue("@IsDefault", installation.IsDefault);
			command.Parameters.AddWithValue("@Order", installation.Order);

			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();
		}
	}

	private static Installation ParseEntity(DbDataReader reader)
	{
		return new Installation
		{
			Id = (int)reader["Id"],
			Name = (string)reader["Name"],
			DeviceId = (int)reader["DeviceId"],
			FilialId = (int)reader["FilialId"],
			IsDefault = (bool)reader["IsDefault"],
			Order = (byte)reader["Order"]
		};
	}
}
