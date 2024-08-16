using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class PurchaseHistoryController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseHistoryController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseHistorory
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.PurchaseHistories.Include(p => p.Usuario);
            return View(await appDBContext.ToListAsync());
        }

        // GET: PurchaseHistorory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseHistory = await _context.PurchaseHistories
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseHistory == null)
            {
                return NotFound();
            }

            return View(purchaseHistory);
        }

        // GET: PurchaseHistorory/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: PurchaseHistorory/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PaymentType,MaskedCardNumber,ProductName,UnitPrice,Qty,TotalPrice,OrderId,Address,Status,UserId")] PurchaseHistory purchaseHistory)
        {
            
                _context.Add(purchaseHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                ViewData["UserId"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", purchaseHistory.UserId);
            
        }

        // GET: PurchaseHistorory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseHistory = await _context.PurchaseHistories.FindAsync(id);
            if (purchaseHistory == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", purchaseHistory.UserId);
            return View(purchaseHistory);
        }

        // POST: PurchaseHistorory/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentType,MaskedCardNumber,ProductName,UnitPrice,Qty,TotalPrice,OrderId,Address,Status,UserId")] PurchaseHistory purchaseHistory)
        {
            if (id != purchaseHistory.Id)
            {
                return NotFound();
            }
                try
                {
                    _context.Update(purchaseHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseHistoryExists(purchaseHistory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            ViewData["UserId"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", purchaseHistory.UserId);
            
        }

        // GET: PurchaseHistorory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseHistory = await _context.PurchaseHistories
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseHistory == null)
            {
                return NotFound();
            }

            return View(purchaseHistory);
        }

        // POST: PurchaseHistorory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseHistory = await _context.PurchaseHistories.FindAsync(id);
            if (purchaseHistory != null)
            {
                _context.PurchaseHistories.Remove(purchaseHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseHistoryExists(int id)
        {
            return _context.PurchaseHistories.Any(e => e.Id == id);
        }
    }
}
