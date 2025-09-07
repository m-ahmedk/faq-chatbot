using FAQDemo.API.Models;
using FAQDemo.API.Services.Interfaces;
using FAQDemo.API.Repositories.Interfaces;

namespace FAQDemo.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Order> PlaceOrderAsync(int userId, List<(int ProductId, int Quantity)> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Order must contain at least one item.");

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending
            };

            foreach (var (productId, quantity) in items)
            {
                var product = await _productRepository.GetByIdAsync(productId)
                    ?? throw new KeyNotFoundException($"Product {productId} not found.");

                if (quantity <= 0)
                    throw new ArgumentException($"Quantity for {product.Name} must be positive.");

                if (quantity > product.Quantity)
                    throw new InvalidOperationException($"Not enough stock for {product.Name}.");

                // reduce stock
                product.Quantity -= quantity;

                order.Items.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            // once stock check passes → mark as confirmed
            order.Status = OrderStatus.Confirmed;

            await _orderRepository.AddAsync(order);

            // reload so Products are included
            var savedOrder = await _orderRepository.GetByIdAsync(order.Id);
            return savedOrder!;
        }

        public Task<Order?> GetOrderByIdAsync(int id)
        {
            return _orderRepository.GetByIdAsync(id);
        }

        public Task<List<Order>> GetOrdersByUserAsync(int userId)
        {
            return _orderRepository.GetByUserIdAsync(userId);
        }

        public async Task<Order> UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            // simple flow enforcement
            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Cannot update a cancelled order.");

            order.Status = newStatus;
            order.LastModifiedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return order;
        }

        public Task<bool> DeleteOrderAsync(int orderId)
        {
            return _orderRepository.DeleteAsync(orderId);
        }
    }
}
