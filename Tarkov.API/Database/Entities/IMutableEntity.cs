namespace Tarkov.API.Database.Entities;

public interface IMutableEntity
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}