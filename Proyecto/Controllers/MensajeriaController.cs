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
    public class MensajeriaController : Controller
    {
        private readonly AppDBContext _context;

        public MensajeriaController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Mensajeria
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contacts.ToListAsync());
        }

        // GET: Mensajeria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactModel = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }

        // GET: Mensajeria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mensajeria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Subject,Message")] ContactModel contactModel)
        {
                _context.Add(contactModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        // GET: Mensajeria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactModel = await _context.Contacts.FindAsync(id);
            if (contactModel == null)
            {
                return NotFound();
            }
            return View(contactModel);
        }

        // POST: Mensajeria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Subject,Message")] ContactModel contactModel)
        {
            if (id != contactModel.Id)
            {
                return NotFound();
            }
                try
                {
                    _context.Update(contactModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactModelExists(contactModel.Id))
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

        // GET: Mensajeria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactModel = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactModel == null)
            {
                return NotFound();
            }

            return View(contactModel);
        }

        // POST: Mensajeria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contactModel = await _context.Contacts.FindAsync(id);
            if (contactModel != null)
            {
                _context.Contacts.Remove(contactModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactModelExists(int id)
        {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}
