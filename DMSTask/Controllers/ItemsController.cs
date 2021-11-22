using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DMSTask.Data;
using DMSTask.Models;
using Microsoft.AspNetCore.Authorization;
using DMSTask.Consts;
using Microsoft.AspNetCore.Identity;
using DMSTask.Services;
using DMSTask.Enums;

namespace DMSTask.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;

        public ItemsController(ApplicationDbContext context,
            IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        #region Admin Actions
        // GET: Items
        [Authorize(Roles = DmsRoles.ADMIN)]
        public async Task<IActionResult> Index(string filter)
        {
            ViewData["Filter"] = filter;

            var applicationDbContext = _context.Items.Include(i => i.UnitOfMeasure).AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter))
            {
                applicationDbContext = applicationDbContext.Where(x =>
                    x.Name!.Contains(filter) ||
                    x.Description!.Contains(filter) ||
                    x.UnitOfMeasure!.UOM!.Contains(filter)
                );
            }

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Items/Details/5
        [Authorize(Roles = DmsRoles.ADMIN)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize(Roles = DmsRoles.ADMIN)]
        public IActionResult Create()
        {
            ViewData["UOM"] = new SelectList(_context.UnitOfMeasures, "Id", "UOM");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = DmsRoles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,UOM,Quantity,Price")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UOM"] = new SelectList(_context.UnitOfMeasures, "Id", "Id", item.UOM);
            return View(item);
        }

        // GET: Items/Edit/5
        [Authorize(Roles = DmsRoles.ADMIN)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            ViewData["UOM"] = new SelectList(_context.UnitOfMeasures, "Id", "UOM", item.UOM);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = DmsRoles.ADMIN)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,UOM,Quantity,Price")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UOM"] = new SelectList(_context.UnitOfMeasures, "Id", "Id", item.UOM);
            return View(item);
        }

        // GET: Items/Delete/5
        [Authorize(Roles = DmsRoles.ADMIN)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [Authorize(Roles = DmsRoles.ADMIN)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.FindAsync(id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Customer Actions
        // GET: AvaliableItems
        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> AvaliableItems()
        {
            var applicationDbContext = _context.Items.Include(i => i.UnitOfMeasure)
                .Where(i => i.Quantity > 0)
                .AsQueryable();

            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> AddToCard(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.UnitOfMeasure)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCardConfirm(int id, int quantity)
        {
            var item = await _context.Items.FindAsync(id);

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == _userService.GetUserId());
            
            var order = await _context.OrderHeaders
                .Include(o => o.OrderDetails.Where(od => od.ItemId == item.Id))
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.CustomerCode &&
                    x.Status == OrderStatus.Open
                );
            
            if(order != null)
            {
                OrderDetail orderDetails = new();
                //If there is OrderDetails for item - update
                if (order.OrderDetails?.Count > 0)
                {
                    orderDetails = order.OrderDetails.First();

                    //Reset Order TotalPrice
                    order.TotalPrice -= orderDetails.TotalPrice;
                    //Re-calculate
                    orderDetails.TotalPrice += CalculatePrice(quantity, item.Price, order.TaxValue, order.DiscountValue);
                    orderDetails.Qty += quantity;
                    orderDetails.ItemPrice = item.Price;
                    orderDetails.Tax = order.TaxValue;
                    orderDetails.Discount = order.DiscountValue;
                    orderDetails.UOM = item.UOM;

                    _context.OrderDetails.Update(orderDetails);
                }
                else //Else - Add
                {
                    orderDetails = new()
                    {
                        OrderId = order.Id,
                        ItemId = item.Id,
                        ItemPrice = item.Price,
                        Qty = quantity,
                        Tax = order.TaxValue,
                        Discount = order.DiscountValue,
                        UOM = item.UOM,
                        TotalPrice = CalculatePrice(quantity, item.Price, order.TaxValue, order.DiscountValue)
                    };

                    _context.OrderDetails.Add(orderDetails);
                }

                order.TotalPrice += orderDetails.TotalPrice;

                item.Quantity -= quantity;
            }
            else
            {
                order = new OrderHeader()
                {
                    CustomerId = customer.CustomerCode,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Open,
                    TaxCode = 1,
                    TaxValue = 0,
                    DiscountCode = 1,
                    DiscountValue = 0,
                    TotalPrice = CalculatePrice(quantity, item.Price, 0, 0),
                    OrderDetails = new List<OrderDetail>()
                    {
                        new OrderDetail()
                        {
                            ItemId = item.Id,
                            ItemPrice = item.Price,
                            Qty = quantity,
                            Tax = 0,
                            Discount = 0,
                            UOM = item.UOM,
                            TotalPrice = CalculatePrice(quantity, item.Price, 0, 0)
                        }
                    }
                };

                _context.OrderHeaders.Add(order);

                item.Quantity -= quantity;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AvaliableItems));
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> Card()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == _userService.GetUserId());

            var order = await _context.OrderHeaders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Item).ThenInclude(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.CustomerCode &&
                    x.Status == OrderStatus.Open
                );

            return View(order);
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> RemoveFromCard(int orderDetailId)
        {
            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order).Include(o => o.Item)
                .FirstOrDefaultAsync(x => x.Id == orderDetailId);

            if(orderDetail == null)
            {
                return NotFound();
            }

            orderDetail.Order.TotalPrice -= orderDetail.TotalPrice;
            orderDetail.Item.Quantity += orderDetail.Qty;

            _context.OrderDetails.Remove(orderDetail);

            var countOfOrderDetails = await _context.OrderDetails.CountAsync(x => x.OrderId == orderDetail.OrderId);
            if(countOfOrderDetails > 0)
            {
                _context.OrderHeaders.Remove(orderDetail.Order);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Card));
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> SaveOrder(int orderId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == _userService.GetUserId());

            var order = await _context.OrderHeaders
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customer.CustomerCode &&
                    x.Status == OrderStatus.Open && 
                    x.Id == orderId
                );

            if (order == null) return NotFound();

            order.Status = OrderStatus.Closed;
            order.DueDate = DateTime.Now;

            _context.OrderHeaders.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AvaliableItems));
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> DisplayMyOrders()
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == _userService.GetUserId());

            var orders = await _context.OrderHeaders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Item).ThenInclude(x => x.UnitOfMeasure)
                .Where(x => x.CustomerId == customer.CustomerCode)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [Authorize(Roles = DmsRoles.CUSTOMER)]
        public async Task<IActionResult> DisplayMyOrderDetails(int orderId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == _userService.GetUserId());

            var order = await _context.OrderHeaders
                .Include(x => x.OrderDetails).ThenInclude(x => x.Item).ThenInclude(x => x.UnitOfMeasure)
                .FirstOrDefaultAsync(x => x.CustomerId == customer.CustomerCode && x.Id == orderId);

            return View(order);
        }
        #endregion

        #region Helper methods
        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }

        private static double CalculatePrice(int quantity, double price, double taxValue, double discountValue)
        {
            return (price * quantity) + taxValue - discountValue;
        }
        #endregion
    }
}
