using MediaVault.Application.Database;
using MediaVault.Application.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaVault.Application.Repositories;

public sealed class CustomerRepository(MediaVaultContext context) : ICustomerRepository
{
    public Task<bool> ExistsAsync(long id, CustomerType type, CancellationToken cancellationToken = default)
    {
        return context.Customers
            .AsNoTracking()
            .AnyAsync(x => x.Id == id && x.Type == type, cancellationToken);
    }
}
