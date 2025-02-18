using Tarkov.API.Database;

namespace Tarkov.API.Application.Tasks;

public interface ISyncTask
{
    public EntitiesCounter EntitiesCounter { get; }

    public Task Run(CancellationToken cancellationToken = default);
}