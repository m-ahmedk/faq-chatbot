using FAQDemo.API.Models;

public interface IOrderRepository
{
    Task<Order> AddAsync(Order order);
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(int userId);
    Task UpdateAsync(Order order);
    Task<bool> DeleteAsync(int id); // soft delete
}