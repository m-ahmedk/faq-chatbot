using FAQDemo.API.Models;

namespace FAQDemo.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> PlaceOrderAsync(int userId, List<(int ProductId, int Quantity)> items);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<List<Order>> GetOrdersByUserAsync(int userId);
        Task<Order> UpdateStatusAsync(int orderId, OrderStatus newStatus);
        Task<bool> DeleteOrderAsync(int orderId);
    }
}