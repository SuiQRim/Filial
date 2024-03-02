namespace PrinterFil.Api.DataBase;

public partial class Filial
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Installation> Installations { get; set; } = new List<Installation>();
}
