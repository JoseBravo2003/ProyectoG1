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
            var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate()); // Rotar la página para mayor ancho
            PdfWriter.GetInstance(document, memoryStream);
            document.Open();

            // Title and User Info
            var titleFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 16);
            var infoFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, 12);
            document.Add(new Paragraph($"User Profile - {user.NombreCompleto}", titleFont));
            document.Add(new Paragraph($"Email: {user.Correo}", infoFont));
            document.Add(new Paragraph($"Password: {user.clave}", infoFont));
            document.Add(new Paragraph("\n"));

            // Purchase History Title
            var purchaseHistoryFont = iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA_BOLD, 14);
            document.Add(new Paragraph("Purchase History", purchaseHistoryFont));
            document.Add(new Paragraph("\n"));

            // Table
            var table = new PdfPTable(9);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 1.5f, 2.5f, 3f, 1.5f, 1f, 1.5f, 1.5f, 3f, 1.5f }); // Set custom column widths
            table.SpacingBefore = 10f; // Space before table
            table.SpacingAfter = 10f; // Space after table
            table.DefaultCell.Padding = 10f; // Increased cell padding

            // Table Headers
            AddCellToTable(table, "Payment Type", true, new BaseColor(200, 200, 200)); // Light gray background
            AddCellToTable(table, "Card Number", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Product Name", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Unit Price", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Qty", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Total Price", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Order ID", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Address", true, new BaseColor(200, 200, 200));
            AddCellToTable(table, "Status", true, new BaseColor(200, 200, 200));

            // Table Data
            var rowColor1 = new BaseColor(255, 255, 255); // White
            var rowColor2 = new BaseColor(240, 240, 240); // Light gray
            var isRowColor1 = true;

            foreach (var purchase in user.PurchaseHistory)
            {
                var rowColor = isRowColor1 ? rowColor1 : rowColor2;
                AddCellToTable(table, purchase.PaymentType, false, rowColor);
                AddCellToTable(table, purchase.MaskedCardNumber, false, rowColor);
                AddCellToTable(table, purchase.ProductName, false, rowColor);
                AddCellToTable(table, purchase.UnitPrice.ToString("C"), false, rowColor);
                AddCellToTable(table, purchase.Qty.ToString(), false, rowColor);
                AddCellToTable(table, purchase.TotalPrice.ToString("C"), false, rowColor);
                AddCellToTable(table, purchase.OrderId, false, rowColor);
                AddCellToTable(table, purchase.Address, false, rowColor);
                AddCellToTable(table, purchase.Status, false, rowColor);

                isRowColor1 = !isRowColor1;
            }

            document.Add(table);
            document.Close();

            var fileBytes = memoryStream.ToArray();
            return File(fileBytes, "application/pdf", "UserProfile.pdf");
        }
    }

    // Helper method to add cells to table with optional header, color, and font
    private void AddCellToTable(PdfPTable table, string text, bool isHeader = false, BaseColor backgroundColor = null, iTextSharp.text.Font font = null)
    {
        var cell = new PdfPCell(new Phrase(text, font ?? iTextSharp.text.FontFactory.GetFont(iTextSharp.text.FontFactory.HELVETICA, isHeader ? 10 : 8)));
        if (isHeader)
        {
            cell.BackgroundColor = backgroundColor ?? new BaseColor(255, 255, 255); // White background for headers
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.BorderWidth = 0.5f; // Thinner border for headers
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 10f; // Increased padding for header cells
        }
        else
        {
            cell.BackgroundColor = backgroundColor ?? BaseColor.WHITE;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.BorderWidth = 0.5f;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 10f; // Increased padding for data cells
            cell.MinimumHeight = 30f; // Minimum height for each cell to increase row height
        }
        table.AddCell(cell);
    }
}
