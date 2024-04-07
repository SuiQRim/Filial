using System.ComponentModel.DataAnnotations;

namespace PFilial.API.Requests;

public record PrintJobRequest(
	string Name,
	int EmployeeId,
	int LayerCount,
	[Range(1, byte.MaxValue)]
	byte? Order);