using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopWeb.Web.Features.MyOrders;
using Microsoft.eShopWeb.Web.Features.OrderDetails;
using System;
using System.Threading.Tasks;
using IronPdf;
using System.Net;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.Web.Controllers.Api
{
    public class OrderUpdate {
        public string Status { get; set; }
        public string Description { get; set; }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize] // Controllers that mainly require Authorization still use Controller/View; other pages use Pages
    [Route("[controller]/[action]")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailSender _emailSender;
    private readonly  IAppLogger<OrderController> _logger;
        public OrderController(
            IAppLogger<OrderController> logger,
            IOrderRepository orderRepository,
            IEmailSender emailSender)
        {
            _logger = logger;
            _orderRepository = orderRepository;
           _emailSender = emailSender;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            await _orderRepository.DeleteAsync(order);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult<Order>> UpdateById(int orderId, [FromBody] OrderUpdate data)
        {
            _logger.LogInformation("Getting order with ID = {order.Id} ");
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) {
                _logger.LogWarning($"Order with ID = {order.Id}  not found");
                return NotFound();
            }
            var oldStatus = order.Status;
            order.Status = data.Status;
            await _orderRepository.UpdateAsync(order);
            if (!oldStatus.Equals(data.Status, StringComparison.CurrentCultureIgnoreCase)) {
                // Notificar utilizador
                _logger.LogInformation("Sending user notification");
                await _emailSender.SendEmailAsync("useremail", $"Order {order.Id} status changed from {oldStatus} to {data.Status}", "message context");
            }
            
            return Ok(order);
        }


    }
}