namespace Tarkov.API.Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string id) : base($"{entityName} with id {id} was not found.")
    {
    }
}