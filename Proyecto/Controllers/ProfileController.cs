using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;

public class ProfileController : Controller
{
    private readonly AppDBContext _context;

    public ProfileController(AppDBContext context)
    {
        _context = context;
    }

    // GET: /Profile/UserProfile
    public async Task<IActionResult> UserProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Usuarios
            .Include(u => u.PurchaseHistory)
            .FirstOrDefaultAsync(u => u.IdUsuario == userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: /Profile/EditUser
    [HttpGet]
    public async Task<IActionResult> EditUser()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Usuarios.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: /Profile/EditUser
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditUser(Usuario model)
    {
     
        
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Usuarios.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.NombreCompleto = model.NombreCompleto;
            user.Correo = model.Correo;

            // Update password only if it's provided and is not empty
            if (!string.IsNullOrWhiteSpace(model.clave))
            {
                user.clave = model.clave; // Consider hashing the password for security
            }

            _context.Usuarios.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("UserProfile");
        

        //return View(model);
    }

    // GET: /Profile/DownloadUserProfilePdf
    public async Task<IActionResult> DownloadUserProfilePdf()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var user = await _context.Usuarios
            .Include(u => u.PurchaseHistory)
            .FirstOrDefaultAsync(u => u.IdUsuario == userId);

        if (user == null)
        {
            return NotFound();
        }

        using (var memoryStream = new MemoryStream())
        {
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            document.Add(new Paragraph($"User Profile - {user.NombreCompleto}"));
            document.Add(new Paragraph($"Email: {user.Correo}"));

            // Do not include password in the PDF for security reasons
            document.Add(new Paragraph($"Password: {user.clave}"));

            document.Add(new Paragraph("Purchase History"));
            var table = new PdfPTable(8);
            table.AddCell("Payment Type");
            table.AddCell("Card Number");
            table.AddCell("Product Name");
            table.AddCell("Unit Price");
            table.AddCell("Qty");
            table.AddCell("Total Price");
            table.AddCell("Order ID");
            table.AddCell("Status");

            foreach (var purchase in user.PurchaseHistory)
            {
                table.AddCell(purchase.PaymentType);
                table.AddCell(purchase.MaskedCardNumber);
                table.AddCell(purchase.ProductName);
                table.AddCell(purchase.UnitPrice.ToString("C")); // Format as currency
                table.AddCell(purchase.Qty.ToString());
                table.AddCell(purchase.TotalPrice.ToString("C")); // Format as currency
                table.AddCell(purchase.OrderId);
                table.AddCell(purchase.Status);
            }

            document.Add(table);
            document.Close();

            var fileBytes = memoryStream.ToArray();
            return File(fileBytes, "application/pdf", "UserProfile.pdf");
        }
    }
}
