namespace MNR.SDK.DataAccess.Models;

public abstract class BaseEntity
{
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; }
    public string DataAreaId { get; set; }
}