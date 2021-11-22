using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DMSTask.Data;
using DMSTask.Models;
using DMSTask.Consts;
using DMSTask.Enums;
using Microsoft.AspNetCore.Authorization;

namespace DMSTask.Controllers
{
    [Authorize(Roles = DmsRoles.ADMIN)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderHeaders
        public async Task<IActionResult> Index(string filter)
        {
            ViewData["Filter"] = filter;

            List<object> orderStatus = GetOrderStatus();

            ViewData["OrderStatus"] = new SelectList(orderStatus, "Id", "Value");

            var applicationDbContext = _context.OrderHeaders.Include(o => o.Customer)
                //.Where(x => x.Status != OrderStatus.Open)
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(filter))
            {
                applicationDbContext = applicationDbContext.Where(x =>
                    x.Customer!.DescriptionAr!.Contains(filter) ||
                    x.Customer!.DescriptionEn!.Contains(filter)
                );
            }

            return View(await applicationDbContext.OrderByDescending(x => x.OrderDate).ToListAsync());
        }

        // GET: OrderHeaders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails).ThenInclude(o => o.Item)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            return View(orderHeader);
        }

        // GET: OrderHeaders/Create
        public IActionResult Create()
        {
            List<object> orderStatus = GetOrderStatus();

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerCode", "DescriptionEn");
            ViewData["OrderStatus"] = new SelectList(orderStatus, "Id", "Value");
            return View();
        }

        // POST: OrderHeaders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CustomerId,OrderDate,DueDate,Status,TaxCode,TaxValue,DiscountCode,DiscountValue,TotalPrice")] OrderHeader orderHeader)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderHeader);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerCode", "CustomerCode", orderHeader.CustomerId);
            return View(orderHeader);
        }

        // GET: OrderHeaders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            List<object> orderStatus = GetOrderStatus();

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerCode", "DescriptionEn");
            ViewData["OrderStatus"] = new SelectList(orderStatus, "Id", "Value");

            return View(orderHeader);
        }

        // POST: OrderHeaders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,OrderDate,DueDate,Status,TaxCode,TaxValue,DiscountCode,DiscountValue,TotalPrice")] OrderHeader orderHeader)
        {
            if (id != orderHeader.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderHeader);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderHeaderExists(orderHeader.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerCode", "CustomerCode", orderHeader.CustomerId);
            return View(orderHeader);
        }

        // GET: OrderHeaders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderHeader = await _context.OrderHeaders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            return View(orderHeader);
        }

        // POST: OrderHeaders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderHeader = await _context.OrderHeaders.FindAsync(id);
            _context.OrderHeaders.Remove(orderHeader);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderHeaderExists(int id)
        {
            return _context.OrderHeaders.Any(e => e.Id == id);
        }

        private static List<object> GetOrderStatus()
        {
            var orderStatus = new List<object>();
            orderStatus.Add(new { Id = (int)OrderStatus.Open, Value = OrderStatus.Open.ToString() });
            orderStatus.Add(new { Id = (int)OrderStatus.Closed, Value = OrderStatus.Closed.ToString() });
            return orderStatus;
        }
    }
}
