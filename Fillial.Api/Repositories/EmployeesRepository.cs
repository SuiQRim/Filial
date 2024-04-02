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
			List<Employee> employees = new();
			string query = "SELECT Id, Name, FilialId FROM Employees";

			using var connection = new SqlConnection(_connectionString);
			using (SqlCommand command = new(query, connection)) {

				await connection.OpenAsync();
				using SqlDataReader reader = await command.ExecuteReaderAsync();
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
