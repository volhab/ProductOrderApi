using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Data.Entities;
using ProductOrderApi.Data.Models;
using ProductOrderApi.Services;

namespace ProductOrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        public OrdersController(OrderService orderService, ProductService productService)
        {
            _productService = productService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var res =  await _orderService.GetOrders();
            return Ok(res.ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var res = await _orderService.GetOrder(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder(CreateOrderModel model)
        {
            var order = new Order();
            order.OrderDate = DateTime.Now;
            order.TotalPrice = 0;
            order.OrderProducts = new List<OrderProduct>();

            foreach (var op in model.OrderProducts)
            {
                var product = await _productService.GetProduct(op.ProductId);
                order.TotalPrice += product.Price * op.Quantity;
                var opm = new OrderProduct
                {
                    Product = product,
                    Price = product.Price,
                    Order = order,
                    ProductId = op.ProductId,
                    Quantity = op.Quantity
                };
                order.OrderProducts.Add(opm);
            }
            return Ok(order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            var item = await _orderService.GetOrder(id);
            if (item == null)
                return NotFound();
            var res = await _orderService.UpdateOrder(order);
            if (res == null)
                return BadRequest();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var res = await _orderService.DeleteOrder(id);
            if (res)
                return NoContent();
            return NotFound();
        }
    }
}
