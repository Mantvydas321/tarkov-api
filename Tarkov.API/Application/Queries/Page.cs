namespace Tarkov.API.Application.Queries;

public class Page<T>
{
    public required int Total { get; set; }
    public required List<T> Items { get; set; }
}