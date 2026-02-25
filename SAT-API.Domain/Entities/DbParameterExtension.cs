namespace SAT_API.Domain.Entities;

 [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ColumnMapAttribute : Attribute
{
    public string? Name { get; }

    public ColumnMapAttribute()
    { 
        Name = string.Empty;
    }

    public ColumnMapAttribute(string name)
    {
        Name = name;
    }
}
