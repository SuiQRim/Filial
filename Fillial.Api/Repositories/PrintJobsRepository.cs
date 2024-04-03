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
			AddToParams(printJob, command);

			await connection.OpenAsync();
			return (int?)await command.ExecuteScalarAsync();
		}

	}

	public async Task CreateRangeAsync(IEnumerable<PrintJob> printJobs)
	{
		string query = "INSERT INTO PrintJobs " +
					   "(Name, EmployeeId, [Order], LayerCount, IsSuccessful) " +
					   "VALUES (@Name, @EmployeeId, @Order, @LayerCount, @IsSuccessful)";

		using (SqlConnection connection = new (_connectionString))
		{
			await connection.OpenAsync();
			using SqlTransaction transaction = connection.BeginTransaction();
			try
			{
				using (SqlCommand command = new(query, connection, transaction))
				{
					foreach (var item in printJobs)
					{
						command.Parameters.Clear();
						AddToParams(item, command);

						await command.ExecuteNonQueryAsync();
					}
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


	private void AddToParams(PrintJob printJob, SqlCommand command)
	{
		command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = printJob.Name;
		command.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = printJob.EmployeeId;
		command.Parameters.Add("@Order", SqlDbType.TinyInt).Value = printJob.Order;
		command.Parameters.Add("@LayerCount", SqlDbType.Int).Value = printJob.LayerCount;
		command.Parameters.Add("@IsSuccessful", SqlDbType.Bit).Value =
			printJob.IsSuccessful == null ? DBNull.Value : printJob.IsSuccessful;
	}
}

