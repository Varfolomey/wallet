using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MNR.SDK.DataAccess.Context;

public class BaseContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("public");
     
        var converter = new ValueConverter<decimal, decimal>(
            a => Calc(a), 
            a => Calc(a));
        
        builder
            .Model
            .GetEntityTypes()
            .Where(e => string.IsNullOrWhiteSpace(e.GetViewName()))
            .SelectMany(t => t.GetProperties())
            .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
            .ToList()
            .ForEach(p =>
            {
                p.SetValueConverter(converter);
            });
        
        base.OnModelCreating(builder);
    }
    
    private static decimal Calc(decimal a)
    {
        var str = a.ToString();
        return decimal.Parse(str.Substring(0, Math.Min(29, str.Length)));
    }
    
    
}