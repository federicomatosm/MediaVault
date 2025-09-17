using MediaVault.Application.Entities;

namespace MediaVault.Application.Repositories;

public interface ICustomerRepository
{
    Task<bool> ExistsAsync(long id, CustomerType type, CancellationToken cancellationToken = default);
}
