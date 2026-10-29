using AspNetCoreCrud.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreCrud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Products.ToListAsync();

            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Product product)
        {

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            
            // Aynı isimde ürün var mı kontrol et
            bool productExists = await _context.Products.AnyAsync(p => p.Title.ToLower() == product.Title.ToLower());

            if (productExists)
            {
                ModelState.AddModelError("Title", "Bu ürün zaten mevcut.");
                return View(product);
            }
            // Finish


            _context.Products.Add(product);
            await _context.SaveChangesAsync();


            TempData["Message"] = "Product Added Successfully.";
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            var existingProduct = await _context.Products.FindAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;

            await _context.SaveChangesAsync();


            TempData["Message"] = $"Product '{product.Title}' updated successfully.";
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Product deleted successfully.";
            return RedirectToAction("Index", "Product", new { area = "Admin" });
        }

    }
}
