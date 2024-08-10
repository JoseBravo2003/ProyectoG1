using Microsoft.AspNetCore.Mvc;
using Proyecto.Data;
using Proyecto.Models;


public class PaymentController : Controller
{
    private readonly AppDBContext _context;

    public PaymentController(AppDBContext context)
    {
        _context = context;
    }

    // Acción GET para mostrar el formulario
    public IActionResult OrderPayment()
    {
        return View();
    }

    // Acción POST para procesar el formulario de tarjeta
    [HttpPost]
    public async Task<IActionResult> Submit(Payment model)
    {
     
            model.PaymentType = "Card"; // Asegúrate de que se establezca el tipo de pago
            _context.Payments.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success"); // Redirige a una página de éxito
        
    }



    // Acción POST para procesar el formulario de cash

    [HttpPost]
    public async Task<IActionResult> SubmitCash(Payment model)
    {
     
            // Asigna valores predeterminados para campos opcionales
            model.CardOwner = model.CardOwner ?? string.Empty;
            model.CardNumber = model.CardNumber ?? string.Empty;
            model.CVV = model.CVV ?? string.Empty;
            model.ExpirationDate = model.ExpirationDate ?? string.Empty;
            model.DeliveryAddress = model.DeliveryAddress ?? string.Empty;

            model.PaymentType = "Cash";
            _context.Payments.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
   
    }

    public IActionResult Success()
    {
        return View();
    }
}
