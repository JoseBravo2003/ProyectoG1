using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Proyecto.Data;
using Proyecto.Models;

namespace Proyecto.Controllers
{
    public class SalesHistoriesController : Controller
    {
        private readonly AppDBContext _context;

        public SalesHistoriesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SalesHistories
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.PurchaseHistories.Include(p => p.Usuario);
            return View(await appDBContext.ToListAsync());
        }

        // GET: SalesHistories/Details/5
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

       
        // Método para descargar el archivo PDF
        public IActionResult DownloadPdf()
        {
            var purchaseHistory = _context.PurchaseHistories.Include(p => p.Usuario).ToList();

            using (MemoryStream stream = new MemoryStream())
            {
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 20f, 20f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();

                // Título del documento
                Font titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                Paragraph title = new Paragraph("Purchase History", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                pdfDoc.Add(title);

                // Tabla
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 3f, 1f, 2f });

                // Encabezados de la tabla
                table.AddCell("Usuario");
                table.AddCell("Product Name");
                table.AddCell("Qty");
                table.AddCell("Total Price");

                // Datos de la tabla
                foreach (var item in purchaseHistory)
                {
                    table.AddCell(item.Usuario.IdUsuario.ToString());
                    table.AddCell(item.ProductName);
                    table.AddCell(item.Qty.ToString());
                    table.AddCell(item.TotalPrice.ToString("C"));
                }

                pdfDoc.Add(table);
                pdfDoc.Close();

                byte[] bytes = stream.ToArray();
                return File(bytes, "application/pdf", "PurchaseHistory.pdf");
            }
        }

        private bool PurchaseHistoryExists(int id)
        {
            return _context.PurchaseHistories.Any(e => e.Id == id);
        }
    }
}
