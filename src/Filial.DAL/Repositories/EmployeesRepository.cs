using PFilial.DAL.Entities;
using Microsoft.Data.SqlClient;
using PFilial.DAL.Repositories.Interfaces;

namespace PrinterFil.Api.Repositories
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly string _connectionString;

		public EmployeesRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<EmployeeEntity[]> ReadAsync()
		{
			List<EmployeeEntity> employees = [];
			string query = "SELECT Id, Name, FilialId FROM Employees";

			await using SqlConnection connection = new(_connectionString);
			await using (SqlCommand command = new(query, connection))
			{
				await connection.OpenAsync();
				await using SqlDataReader reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					employees.Add(new EmployeeEntity
					{
						Id = (int)reader["Id"],
						Name = (string)reader["Name"],
						FilialId = (int)reader["FilialId"]
					});
				}
			}
			return employees.ToArray();
		}
	}
}
