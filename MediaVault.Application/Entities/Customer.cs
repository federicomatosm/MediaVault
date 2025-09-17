using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaVault.Application.Entities;

public sealed class Customer
{
   
    public long Id { get; init; }
    
    [MaxLength(255)] 
    public string Name { get; init; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public CustomerType Type { get; init; }

    public ICollection<ProfileImage> Images { get; init; } = new HashSet<ProfileImage>();
}

public enum CustomerType
{
    Lead,
    Customer
}