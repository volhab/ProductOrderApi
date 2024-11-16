﻿using Microsoft.EntityFrameworkCore;
using ProductOrderApi.Data.Entities;

namespace ProductOrderApi.Data.Repositories
{
    public class OrderRepository
    {
        private readonly OrderContext _context;
        public OrderRepository(OrderContext context)
        {
            _context = context;
        }
        public async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();
        }
        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }
        public async Task<Order> UpdateOrderAsync(Order order)
        {
            var item = _context.Orders.SingleOrDefault(p => p.Id == order.Id);
            if (item == null)
            {
                return null;
            }
            item.OrderDate = order.OrderDate;
            item.TotalPrice = order.TotalPrice;
            _context.Orders.Update(item);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return false;
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
