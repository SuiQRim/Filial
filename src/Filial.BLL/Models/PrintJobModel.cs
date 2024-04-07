namespace PFilial.BLL.Models;

public record PrintJobModel(
	int Id,
	string Name,
	int EmployeeId,
	byte? InstallationOrder,
	int LayerCount);
