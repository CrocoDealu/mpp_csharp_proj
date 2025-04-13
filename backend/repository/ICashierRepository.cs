using backend.model;

namespace backend.repository;

public interface ICashierRepository: IRepository<int, Cashier>
{
    public Cashier? FindByUsername(String username);
}