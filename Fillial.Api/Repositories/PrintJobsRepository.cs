using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;
using System.Data;

namespace PrinterFil.Api.Repositories;

public class PrintJobsRepository : IPrintJobsRepository
{
	private readonly string _connectionString;
    public PrintJobsRepository(string connectionString)
    {
		_connectionString = connectionString;
	}

    public async Task<int?> CreateAsync(PrintJob printJob)
	{
		string query = "INSERT INTO PrintJobs " +
				"(Name, EmployeeId, [Order], LayerCount, IsSuccessful) output INSERTED.ID " +
				"VALUES (@Name, @EmployeeId, @Order, @LayerCount, @IsSuccessful)";
		using SqlConnection connection = new(_connectionString);
		using (SqlCommand command = new(query, connection)) 
		{
			command.Parameters.AddWithValue("@Name", printJob.Name);
			command.Parameters.AddWithValue("@EmployeeId", printJob.EmployeeId);
			command.Parameters.AddWithValue("@Order", printJob.Order);
			command.Parameters.AddWithValue("@LayerCount", printJob.LayerCount);
			command.Parameters.AddWithValue("@IsSuccessful", 
				printJob.IsSuccessful == null ? DBNull.Value : printJob.IsSuccessful);

			await connection.OpenAsync();
			return (int?)await command.ExecuteScalarAsync();
		}

	}

	public async Task CreateRangeAsync(IEnumerable<PrintJob> printJobs)
	{
		string query = "INSERT INTO PrintJobs " +
				"(Name, EmployeeId, [Order], LayerCount, IsSuccessful) " +
				"VALUES (@Name, @EmployeeId, @Order, @LayerCount, @IsSuccessful)";
		using SqlConnection connection = new(_connectionString);
		using SqlCommand command = new(query, connection);
		using (SqlDataAdapter adapter = new())
		{
			command.Parameters.Add("@Name", SqlDbType.NVarChar);
			command.Parameters.Add("@EmployeeId", SqlDbType.Int);
			command.Parameters.Add("@Order", SqlDbType.TinyInt);
			command.Parameters.Add("@LayerCount", SqlDbType.Int);
			command.Parameters.Add("@IsSuccessful", SqlDbType.Bit);

			await connection.OpenAsync();
			SqlTransaction transaction = connection.BeginTransaction();
			command.Transaction = transaction;
			try
			{
				foreach (var item in printJobs)
				{
					command.Parameters["@Name"].Value = item.Name;
					command.Parameters["@EmployeeId"].Value = item.EmployeeId;
					command.Parameters["@Order"].Value = item.Order;
					command.Parameters["@LayerCount"].Value = item.LayerCount;
					command.Parameters["@IsSuccessful"].Value = 
						item.IsSuccessful == null ? DBNull.Value : item.IsSuccessful;

					await command.ExecuteNonQueryAsync();
				}

				await transaction.CommitAsync();
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}

