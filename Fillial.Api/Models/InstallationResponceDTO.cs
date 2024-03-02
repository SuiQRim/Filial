﻿namespace PrinterFil.Api.Models
{
	/// <summary>
	/// Инсталляция
	/// </summary>
	/// <param name="Id">Идентификатор</param>
	/// <param name="Name">Название</param>
	/// <param name="FilialId">Идентификатор филиала</param>
	/// <param name="PrintingDeviceId">Идентификатор устройства печати</param>
	/// <param name="IsDefault">Признак "по умолчанию" для своего филиала</param>
	/// <param name="Order">Порядковый номер</param>
	public record InstallationResponceDTO(int Id, string Name, int FilialId, int PrintingDeviceId, bool IsDefault, int Order);
}
