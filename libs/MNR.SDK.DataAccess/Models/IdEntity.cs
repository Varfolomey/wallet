using System.ComponentModel.DataAnnotations;

namespace MNR.SDK.DataAccess.Models;

public class IdEntity<TId> : BaseEntity
{
    [Key]
    public TId Id { get; set; }
}