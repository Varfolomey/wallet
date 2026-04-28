using System.Runtime.Serialization;

namespace MNR.SDK.Commons.Models.PagedRequest;

[DataContract]
public class SortDescriptor
{
    public SortDescriptor(string field, EnumSortDirection order = EnumSortDirection.Asc)
    {
        Field = field;
        Order = order;
    }

    [DataMember(Order = 1)] public string Field { get; set; }
    [DataMember(Order = 2)] public EnumSortDirection Order { get; set; }

    public override string ToString()
    {
        var direction = string.Empty;

        if (Order == EnumSortDirection.Desc) direction = $" {Order.ToString()}";

        return $"{Field}{direction}";
    }
}