﻿using Microsoft.Data.SqlClient;
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
			using (var connection = new SqlConnection(_connectionString))
			{
				await connection.OpenAsync();
				var command = new SqlCommand("SELECT * FROM Employees", connection);

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
