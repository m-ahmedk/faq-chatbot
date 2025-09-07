using AutoMapper;
using FAQDemo.API.Models;
using FAQDemo.API.Services.Interfaces;
using FAQDemo.API.DTOs.Order;
using FAQDemo.API.Helpers;
using FAQDemo.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FAQDemo.API.Controllers
{
    [Authorize] // requires login
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public OrdersController(IOrderService orderService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _currentUserService = currentUserService;
            _orderService = orderService;
            _mapper = mapper;
        }

        // Place a new order
        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderDto dto)
        {
            if (_currentUserService.UserId == null)
                return Unauthorized(ApiResponse<string>.FailResponse("User not logged in"));

            var items = dto.Items.Select(i => (i.ProductId, i.Quantity)).ToList();
            var order = await _orderService.PlaceOrderAsync(_currentUserService.UserId.Value, items);
            var orderDto = _mapper.Map<OrderDto>(order);

            return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order placed successfully"));
        }

        // Get order by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<string>.FailResponse("Order not found"));

            var orderDto = _mapper.Map<OrderDto>(order);
            return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto));
        }

        // Get all orders for current user
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            if (_currentUserService.UserId == null)
                return Unauthorized(ApiResponse<string>.FailResponse("User not logged in"));

            var orders = await _orderService.GetOrdersByUserAsync(_currentUserService.UserId.Value);
            var orderDtos = _mapper.Map<List<OrderDto>>(orders);

            return Ok(ApiResponse<List<OrderDto>>.SuccessResponse(orderDtos));
        }

        // Update order status (Admin or system agent use case)
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")] // only admins/agents can change status
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] OrderStatus newStatus)
        {
            var order = await _orderService.UpdateStatusAsync(id, newStatus);
            var orderDto = _mapper.Map<OrderDto>(order);

            return Ok(ApiResponse<OrderDto>.SuccessResponse(orderDto, "Order status updated"));
        }

        // Soft delete (cancel order)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var deleted = await _orderService.DeleteOrderAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.FailResponse("Order not found"));

            return Ok(ApiResponse<string>.SuccessResponse("Order cancelled successfully"));
        }
    }
}
