using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using back_end.domain.Entities;
using back_end.domain.Enums;

namespace back_end.domain.Seeders
{
    /// <summary>
    /// Seeds OrderItem entries based on wave patterns and menu/category.
    /// </summary>
    public class OrderItemSeeder : ISeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderItemSeeder> _logger;
        private readonly Random _rng = new();

        public OrderItemSeeder(ApplicationDbContext context, ILogger<OrderItemSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Seed()
        {
            var orders = _context.SessionOrders.ToList();
            if (!orders.Any())
                throw new InvalidOperationException("No orders found. Seed session orders first.");

            var items = _context.MenuItems.Include(mi => mi.Category).ToList();
            if (!items.Any())
                throw new InvalidOperationException("No menu items found. Seed menu items first.");

            // group items by category name
            var byCategory = items.GroupBy(i => i.Category.Name)
                                  .ToDictionary(g => g.Key, g => g.ToList());

            // lookup current prices from assignments
            var assignments = _context.MenuItemAssignments.ToList();
            var price = assignments.ToDictionary(a => (a.MenuId, a.ItemId), a => a.Price);

            int created = 0;

            var wavePatterns = new Dictionary<string, (int min, int max)[]>
            {
                ["first_wave"] = new[] {
                    Cat("Appetizers", 2, 4), Cat("Sushi Rolls", 2, 3), Cat("Hot Dishes", 1, 2), Cat("Nigiri", 2, 3), Cat("Sashimi",1,2)
                },
                ["middle_wave"] = new[] {
                    Cat("Sushi Rolls", 3, 5), Cat("Hot Dishes", 2, 3), Cat("Nigiri",3,4), Cat("Sashimi",2,3), Cat("Appetizers",1,2)
                },
                ["last_wave"] = new[] {
                    Cat("Desserts",1,2), Cat("Sushi Rolls",1,2), Cat("Nigiri",1,2)
                }
            };

            foreach (var order in orders)
            {
                var sameBillOrders = _context.SessionOrders
                    .Where(o => o.BillId == order.BillId)
                    .OrderBy(o => o.CreatedAt)
                    .ToList();

                var idx = sameBillOrders.FindIndex(o => o.OrderId == order.OrderId);
                var pattern = idx == 0 ? "first_wave" : (idx == sameBillOrders.Count - 1 ? "last_wave" : "middle_wave");

                foreach (var (category, min, max) in wavePatterns[pattern])
                {
                    if (!byCategory.ContainsKey(category)) continue;

                    var num = _rng.Next(min, max + 1);
                    var chosen = byCategory[category].OrderBy(_ => _rng.Next()).Take(Math.Min(num, byCategory[category].Count)).ToList();

                    foreach (var item in chosen)
                    {
                        var unitPrice = price.GetValueOrDefault((1, item.ItemId), 0m); // default to menu 1

                        int qty;
                        if (category is "Sashimi" or "Nigiri")
                            qty = _rng.Next(2, 5);
                        else if (category == "Desserts")
                            qty = 1;
                        else
                            qty = _rng.Next(1, 3);

                        var oi = new OrderItem
                        {
                            OrderId = order.OrderId,
                            MenuId = 1,
                            ItemId = item.ItemId,
                            Quantity = qty,
                            PriceAtTime = unitPrice,
                            Status = order.Status,
                            CompletedAt = order.CompletedAt
                        };

                        _context.OrderItems.Add(oi);
                        created++;

                        if (created % 100 == 0) _context.SaveChanges();
                    }
                }
            }

            _context.SaveChanges();

            var totalPending = _context.OrderItems.Count(i => i.Status == OrderStatus.Pending);
            var totalCompleted = _context.OrderItems.Count(i => i.Status == OrderStatus.Completed);
            _logger.LogInformation($"Created {created} order items ({totalPending} pending, {totalCompleted} completed)");

            static (string cat, int min, int max) Cat(string c, int a, int b) => (c, a, b);
        }
    }
}