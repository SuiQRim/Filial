namespace PrinterFil.Api.Exceptions;

public class EntityNotFoundExceptions : Exception
{
    public EntityNotFoundExceptions()
    {
            
    }
    public EntityNotFoundExceptions(string message) : base(message) { }
	public EntityNotFoundExceptions(Type type, string id) :
        base($"Entity ({type.Name}) with Id {id} is Not Found")
    { 
    }
}

