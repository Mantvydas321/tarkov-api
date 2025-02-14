namespace Tarkov.API.Application.Tasks;

public interface ISyncTask
{
    public Task Run(CancellationToken cancellationToken = default);
}