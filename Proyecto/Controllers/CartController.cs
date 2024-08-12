using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using Proyecto.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Proyecto.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDBContext _context;

        public CartController(AppDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtén el ID del usuario autenticado
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Obtén los ítems del carrito para el usuario autenticado
            var cartItems = await _context.Carts
                                          .Include(sci => sci.Comida) // Incluye la información del producto
                                          .Where(sci => sci.UserId == int.Parse(userId))
                                          .ToListAsync();

            // Calcula el total del carrito
            var total = cartItems.Sum(ci => ci.Quantity * ci.UnitPrice);

            // Pasa los ítems del carrito y el total a la vista
            ViewBag.Total = total;

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int comidaId)
        {

            Console.WriteLine($"comidaId: {comidaId}");
            // Obtén el ID del usuario autenticado
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            

            // Busca si ya existe un ítem de este producto en el carrito
            var existingCartItem = _context.Carts
                                           .FirstOrDefault(sci => sci.IdComida == comidaId && sci.UserId == userId);

            if (existingCartItem != null)
            {
                // Si ya existe, incrementa la cantidad
                existingCartItem.Quantity++;
            }
            else
            {
                // Si no existe, crea un nuevo ítem en el carrito
                var comida = _context.Comidas.Find(comidaId);
                var newCartItem = new Cart
                {
                    IdComida = comidaId,
                    UserId = userId,
                    Quantity = 1,
                    UnitPrice = comida.Precio
                };

                _context.Carts.Add(newCartItem);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", "Comidas"); // Redirige a la vista de Comida o donde lo necesites
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int Id)
        {
            var cartItem = _context.Carts.Find(Id);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult UpdateCart(int Id, string action)
        {
            var cartItem = _context.Carts.Find(Id);

            if (cartItem != null)
            {
                if (action == "increment")
                {
                    cartItem.Quantity++;
                }
                else if (action == "decrement")
                {
                    cartItem.Quantity--;
                    if (cartItem.Quantity <= 0)
                    {
                        _context.Carts.Remove(cartItem);
                    }
                }

                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }


       




    }



}
