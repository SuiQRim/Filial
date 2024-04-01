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
		using (SqlConnection connection = new (_connectionString))
		{
			await connection.OpenAsync();

			SqlCommand command = new("SELECT * FROM Installations WHERE FilialId = COALESCE(@FilialId, FilialId)", connection);
			command.Parameters.Add(new SqlParameter("@FilialId", filialId.HasValue ? filialId : DBNull.Value));

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
		using SqlConnection connection = new (_connectionString);
		await connection.OpenAsync();

		SqlCommand command = new("SELECT * FROM Installations WHERE Id = @Id", connection);
		command.Parameters.Add(new SqlParameter("@Id", id));

		using SqlDataReader reader = await command.ExecuteReaderAsync();

		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public async Task<Installation?> ReadDefaultAsync(int filialId)
	{
		using SqlConnection connection = new(_connectionString);
		await connection.OpenAsync();

		SqlCommand command = new("SELECT * FROM Installations WHERE FilialId = @FilialId AND IsDefault = 1", connection);
		command.Parameters.Add(new SqlParameter("@FilialId", filialId));

		using SqlDataReader reader = await command.ExecuteReaderAsync();

		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public async Task<Installation?> ReadFirstAsync(int filialId)
	{
		using SqlConnection connection = new(_connectionString);
		await connection.OpenAsync();

		SqlCommand command = new("SELECT TOP 1 * FROM Installations WHERE FilialId = @FilialId", connection);
		command.Parameters.Add(new SqlParameter("@FilialId", filialId));

		using SqlDataReader reader = await command.ExecuteReaderAsync();

		return await reader.ReadAsync() ? ParseEntity(reader) : null;
	}

	public Task CreateAsync(Installation installation)
	{
		throw new NotImplementedException();
	}

	public Task DeleteAsync(int id)
	{
		throw new NotImplementedException();
	}

	public Task<bool> Exist(int? filialId = null, byte? order = null)
	{
		throw new NotImplementedException();
	}

	public Task<byte> GetOrderAsync(int filialId)
	{
		throw new NotImplementedException();
	}

	public Task SaveChangesAsync()
	{
		throw new NotImplementedException();
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
