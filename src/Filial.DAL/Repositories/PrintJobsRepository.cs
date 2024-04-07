using Microsoft.Data.SqlClient;
using PFilial.DAL.Entities;
using PFilial.DAL.Repositories.Interfaces;
using System.Data;

namespace PFilial.DAL.Repositories;

public class PrintJobsRepository : IPrintJobsRepository
{
	private readonly string _connectionString;
	public PrintJobsRepository(string connectionString)
	{
		_connectionString = connectionString;
	}

	public async Task<int?> CreateAsync(PrintJobEntity printJob)
	{
		string query = "INSERT INTO PrintJobs " +
				"(Name, EmployeeId, [Order], LayerCount, IsSuccessful) output INSERTED.ID " +
				"VALUES (@Name, @EmployeeId, @Order, @LayerCount, @IsSuccessful)";

		await using SqlConnection connection = new(_connectionString);
		await using SqlCommand command = new(query, connection);

		command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = printJob.Name;
		command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = printJob.EmployeeId;
		command.Parameters.Add("@Order", SqlDbType.TinyInt).Value = printJob.Order;
		command.Parameters.Add("@LayerCount", SqlDbType.Int).Value = printJob.LayerCount;
		command.Parameters.Add("@IsSuccessful", SqlDbType.Bit).Value =
			printJob.IsSuccessful == null ? DBNull.Value : printJob.IsSuccessful;

		await connection.OpenAsync();
		return (int?)await command.ExecuteScalarAsync();

	}

	public async Task<int> CreateRangeAsync(IEnumerable<PrintJobEntity> printJobs)
	{
		string query = "INSERT INTO PrintJobs " +
					   "(Name, EmployeeId, [Order], LayerCount, IsSuccessful) " +
					   "VALUES (@Name, @EmployeeId, @Order, @LayerCount, @IsSuccessful)";

		await using SqlConnection connection = new(_connectionString);
		await connection.OpenAsync();
		await using SqlTransaction transaction = connection.BeginTransaction();
		try
		{
			int count = 0;
			await using (SqlCommand command = new(query, connection, transaction))
			{
				command.Parameters.Add("@Name", SqlDbType.NVarChar);
				command.Parameters.Add("@EmployeeId", SqlDbType.Int);
				command.Parameters.Add("@Order", SqlDbType.TinyInt);
				command.Parameters.Add("@LayerCount", SqlDbType.Int);
				command.Parameters.Add("@IsSuccessful", SqlDbType.Bit);

				foreach (var item in printJobs)
				{
					command.Parameters["@Name"].Value = item.Name;
					command.Parameters["@EmployeeId"].Value = item.EmployeeId;
					command.Parameters["@Order"].Value = item.Order;
					command.Parameters["@LayerCount"].Value = item.LayerCount;
					command.Parameters["@IsSuccessful"].Value =
						item.IsSuccessful == null ? DBNull.Value : item.IsSuccessful;

					count += await command.ExecuteNonQueryAsync();
				}
			}
			await transaction.CommitAsync();
			return count;
		}
		catch (Exception)
		{
			await transaction.RollbackAsync();
			throw;
		}
	}
}

