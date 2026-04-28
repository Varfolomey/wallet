namespace MNR.SDK.DataAccess.Models;

public abstract class BusinessEntity : BaseEntity
{
    public string CustomerId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}