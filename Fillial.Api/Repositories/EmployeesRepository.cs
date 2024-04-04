using Microsoft.Data.SqlClient;
using PrinterFil.Api.DataBase;
using PrinterFil.Api.Repositories.IRepositories;

namespace PrinterFil.Api.Repositories
{
	public class EmployeesRepository : IEmployeesRepository
	{
		private readonly string _connectionString;

		public EmployeesRepository(string connectionString)
		{
			_connectionString = connectionString;
		}

		public async Task<IEnumerable<Employee>> ReadAsync()
		{
			List<Employee> employees = [];
			string query = "SELECT Id, Name, FilialId FROM Employees";

			await using SqlConnection connection = new(_connectionString);
			await using (SqlCommand command = new(query, connection))
			{
				await connection.OpenAsync();
				await using SqlDataReader reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					employees.Add(new Employee
					{
						Id = (int)reader["Id"],
						Name = (string)reader["Name"],
						FilialId = (int)reader["FilialId"]
					});
				}
			}
			return employees;
		}
	}
}
