using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using Proyecto.Models;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;

public class PaymentController : Controller
{
    private readonly AppDBContext _context;

    public PaymentController(AppDBContext context)
    {
        _context = context;
    }

    // Acción GET para mostrar el formulario de pago
    public IActionResult OrderPayment()
    {
        return View();
    }

    // Acción POST para procesar el formulario de tarjeta
    [HttpPost]
    public async Task<IActionResult> Submit(Payment model)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        model.PaymentType = "Card"; 
        _context.Payments.Add(model);
        await _context.SaveChangesAsync();

        // Obtener los ítems del carrito del usuario
        var cartItems = _context.Carts
                        .Include(sci => sci.Comida)
                        .Where(sci => sci.UserId == userId)
                        .ToList();

        // Enmascarar el número de tarjeta sin lanzar excepciones
        string maskedCardNumber = "**** **** **** ";
        if (!string.IsNullOrEmpty(model.CardNumber) && model.CardNumber.Length >= 4)
        {
            maskedCardNumber += model.CardNumber.Substring(model.CardNumber.Length - 4);
        }
        else
        {
            maskedCardNumber += "****";  // O un valor predeterminado para representar una tarjeta inválida
        }


        // Generar un ID de orden único
        var orderId = Guid.NewGuid().ToString();

        // Crear un registro de PurchaseHistory por cada ítem en el carrito
        foreach (var item in cartItems)
        {
            var purchaseHistory = new PurchaseHistory
            {
                UserId = userId,
                PaymentType = model.PaymentType,
                MaskedCardNumber = maskedCardNumber,
                ProductName = item.Comida.Nombre,
                UnitPrice = item.UnitPrice,
                Qty = item.Quantity,
                TotalPrice = item.Quantity * item.UnitPrice,
                OrderId = orderId,
                Address = model.DeliveryAddress,
                Status = "Pendiente"
            };

            _context.PurchaseHistories.Add(purchaseHistory);
        }

        await _context.SaveChangesAsync();

        // Vaciar el carrito de compras
        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("Success");
    }

    // Acción POST para procesar el formulario de cash
    [HttpPost]
    public async Task<IActionResult> SubmitCash(Payment model)
    {
        var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

        // Asigna valores predeterminados para campos opcionales
        model.CardOwner = model.CardOwner ?? string.Empty;
        model.CardNumber = model.CardNumber ?? string.Empty;
        model.CVV = model.CVV ?? string.Empty;
        model.ExpirationDate = model.ExpirationDate ?? string.Empty;
        model.DeliveryAddress = model.DeliveryAddress ?? string.Empty;

        model.PaymentType = "Cash";
        _context.Payments.Add(model);
        await _context.SaveChangesAsync();

        // Obtener los ítems del carrito del usuario
        var cartItems = _context.Carts
                                .Include(sci => sci.Comida) // Asegúrate de incluir la navegación de 'Comida'
                                .Where(sci => sci.UserId == userId)
                                .ToList();

        // Generar un ID de orden único
        var orderId = Guid.NewGuid().ToString();

        // Crear un registro de PurchaseHistory por cada ítem en el carrito
        foreach (var item in cartItems)
        {
            var purchaseHistory = new PurchaseHistory
            {
                UserId = userId,
                PaymentType = model.PaymentType,
                MaskedCardNumber = "N/A",  // No es necesario para efectivo
                ProductName = item.Comida?.Nombre ?? "Producto desconocido", // Manejar posible nulo
                UnitPrice = item.UnitPrice,
                Qty = item.Quantity,
                TotalPrice = item.Quantity * item.UnitPrice,
                OrderId = orderId,
                Address = model.DeliveryAddress,
                Status = "Pendiente"
            };

            _context.PurchaseHistories.Add(purchaseHistory);
        }

        await _context.SaveChangesAsync();

        // Vaciar el carrito de compras
        _context.Carts.RemoveRange(cartItems);
        await _context.SaveChangesAsync();

        return RedirectToAction("Success");
    }


    public IActionResult Success()
    {
        return View();
    }
}
