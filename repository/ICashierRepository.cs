using Avalonia.Data;
using ConsoleApp1.model;

namespace ConsoleApp1.repository;

public interface ICashierRepository: IRepository<int, Cashier>
{
    public Cashier? FindByUsername(String username);
}