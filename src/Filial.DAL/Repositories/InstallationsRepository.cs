using Microsoft.Data.SqlClient;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;
using System.Data.Common;

namespace PFilial.DAL.Repositories;

public class InstallationsRepository : IInstallationsRepository
{
	private readonly string _connectionString;
	public InstallationsRepository(string connection)
	{
		_connectionString = connection;
	}

	public async Task<InstallationEntity[]> ReadAsync(int? filialId)
	{
		List<InstallationEntity> installations = [];
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = COALESCE(@FilialId, FilialId)";

		await using (SqlConnection connection = new(_connectionString))
		{
			await using SqlCommand command = new(query, connection);

			command.Parameters.AddWithValue("@FilialId", filialId.HasValue ? filialId : DBNull.Value);

			await connection.OpenAsync();
			await using SqlDataReader reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				installations.Add(ReadEntity(reader));
			}
		}
		return installations.ToArray();
	}

	public async Task<InstallationEntity?> ReadAsync(int id)
	{
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE Id = @Id";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@Id", id);

		await connection.OpenAsync();
		using SqlDataReader reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync() ? ReadEntity(reader) : null;
	}

	public async Task<InstallationEntity?> ReadDefaultAsync(int filialId)
	{
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = @FilialId AND IsDefault = 1";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@FilialId", filialId);

		await connection.OpenAsync();
		using SqlDataReader reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync() ? ReadEntity(reader) : null;
	}


	public async Task<InstallationEntity?> ReadByOrderAsync(int filialId, byte order)
	{
		string query = "SELECT Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
				"FROM Installations WHERE FilialId = @FilialId AND [Order] = @Order";

		await using SqlConnection connection = new(_connectionString);
		SqlCommand command = new(query, connection);
		command.Parameters.AddWithValue("@FilialId", filialId);
		command.Parameters.AddWithValue("@Order", order);

		await connection.OpenAsync();
		await using SqlDataReader reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync() ? ReadEntity(reader) : null;
	}

	public async Task<InstallationEntity?> ReadFirstAsync(int filialId)
	{
		string query = "SELECT TOP 1 Id, Name, DeviceId, FilialId, IsDefault, [Order] " +
			"FROM Installations WHERE FilialId = @FilialId";
		using SqlConnection connection = new(_connectionString);

		SqlCommand command = new(query, connection);
		command.Parameters.AddWithValue("@FilialId", filialId);

		await connection.OpenAsync();
		await using SqlDataReader reader = await command.ExecuteReaderAsync();
		return await reader.ReadAsync() ? ReadEntity(reader) : null;
	}

	public async Task<int?> CreateAsync(InstallationEntity installation)
	{
		string query = "INSERT INTO Installations (Name, FilialId, DeviceId, IsDefault, [Order]) output INSERTED.ID " +
			"VALUES (@Name, @FilialId, @DeviceId, @IsDefault, @Order)";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		AddToParams(installation, command);

		await connection.OpenAsync();
		return (int?)await command.ExecuteScalarAsync();
	}

	public async Task UpdateAsync(int id, InstallationEntity installation)
	{
		string query = "UPDATE Installations SET " +
			"Name = @Name, FilialId = @FilialId, DeviceId = @DeviceId, IsDefault = @IsDefault, [Order] = @Order " +
			"WHERE Id = @Id";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@Id", id);
		AddToParams(installation, command);

		await connection.OpenAsync();
		await command.ExecuteNonQueryAsync();
	}

	private static void AddToParams(InstallationEntity installation, SqlCommand command)
	{
		command.Parameters.AddWithValue("@Name", installation.Name);
		command.Parameters.AddWithValue("@FilialId", installation.FilialId);
		command.Parameters.AddWithValue("@DeviceId", installation.DeviceId);
		command.Parameters.AddWithValue("@IsDefault", installation.IsDefault);
		command.Parameters.AddWithValue("@Order", installation.Order);
	}

	public async Task DeleteAsync(int id)
	{
		string query = "DELETE FROM Installations WHERE Id = @Id";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@Id", id);

		await connection.OpenAsync();
		await command.ExecuteNonQueryAsync();
	}

	public async Task<byte?> GetOrderAsync(int filialId)
	{
		const byte maxValue = byte.MaxValue;
		// Запрос вернет минимальное не использованное число от 1 до 255
		string query = "SELECT MIN([Order]) + 1 FROM Installations " +
			"WHERE [Order] < @MaxValue AND [Order] + 1 NOT IN (" +
			"SELECT [Order] FROM Installations WHERE FilialId = @FilialId)";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.AddWithValue("@MaxValue", maxValue);
		command.Parameters.AddWithValue("@FilialId", filialId);

		await connection.OpenAsync();
		object? result = await command.ExecuteScalarAsync();

		return !Convert.IsDBNull(result) ? Convert.ToByte(result) : null;
	}

	private static InstallationEntity ReadEntity(DbDataReader reader)
	{
		return new InstallationEntity
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
