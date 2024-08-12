using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Proyecto.Controllers
{
    public class ComidasController : Controller
    {
        private readonly AppDBContext _context;

        public ComidasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Comidas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comidas.ToListAsync());
        }

        // GET: Comidas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comida = await _context.Comidas
                .FirstOrDefaultAsync(m => m.IdComida == id);
            if (comida == null)
            {
                return NotFound();
            }

            return View(comida);
        }

        // GET: Comidas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comidas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdComida,Nombre,Descripcion,Precio,Categoria,ImagenUrl,Cantidad")] Comida comida)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comida);
        }

        // GET: Comidas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comida = await _context.Comidas.FindAsync(id);
            if (comida == null)
            {
                return NotFound();
            }
            return View(comida);
        }

        // POST: Comidas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdComida,Nombre,Descripcion,Precio,Categoria,ImagenUrl,Cantidad")] Comida comida)
        {
            if (id != comida.IdComida)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comida);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComidaExists(comida.IdComida))
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
            return View(comida);
        }

        // GET: Comidas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comida = await _context.Comidas
                .FirstOrDefaultAsync(m => m.IdComida == id);
            if (comida == null)
            {
                return NotFound();
            }

            return View(comida);
        }

        // POST: Comidas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comida = await _context.Comidas.FindAsync(id);
            _context.Comidas.Remove(comida);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            // Buscar el producto en la tabla de Comidas
            var product = _context.Comidas.FirstOrDefault(p => p.IdComida == productId);
            if (product != null)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                // Crear una nueva entrada en PurchaseHistory
                var cartItem = new PurchaseHistory
                {
                    UserId = userId,
                    ProductName = product.Nombre,  // Usar la propiedad correcta de Comida
                    UnitPrice = product.Precio,
                    Qty = 1,
                    TotalPrice = product.Precio,  // TotalPrice es igual al precio aquí
                    OrderId = Guid.NewGuid().ToString(), // Generar un nuevo ID de orden
                    Status = "InCart"
                };

                _context.PurchaseHistories.Add(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Cart"); // Redirige a la vista del carrito
        }


        private bool ComidaExists(int id)
        {
            return _context.Comidas.Any(e => e.IdComida == id);
        }
    }
}
