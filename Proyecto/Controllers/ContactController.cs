using Microsoft.AspNetCore.Mvc;
using Proyecto.Models;  // Asegúrate de que el namespace sea correcto
using Proyecto.Data;    // Asegúrate de que el namespace para AppDBContext sea correcto

namespace Proyecto.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDBContext _context;

        public ContactController(AppDBContext context)
        {
            _context = context;
        }

        // GET: /Contact
        public IActionResult Contact()
        {
            return View();
        }

        // POST: /Contact
        [HttpPost]
        public async Task<IActionResult> Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Contacts.Add(model);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Your message has been sent successfully.";
                return RedirectToAction("Contact");
            }

            return View(model);
        }
    }
}
