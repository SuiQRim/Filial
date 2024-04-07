namespace PFilial.BLL.Models;

public record PrintJobDTO(
	string Name,
	int EmployeeId,
	byte InstallationOrder,
	int LayerCount);
