using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KitapKategoriController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KitapKategoriController> _logger;

        public KitapKategoriController(ApplicationDbContext context, ILogger<KitapKategoriController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: KitapKategori
        public async Task<IActionResult> Index()
        {
            var kategoriler = await _context.KitapKategoriler.ToListAsync();
            return View(kategoriler);
        }

        // GET: KitapKategori/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: KitapKategori/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KitapKategori kitapKategori)
        {
            _logger.LogInformation("Create metodu çağrıldı. Kategori adı: {KategoriAdi}", kitapKategori.Ad);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.KitapKategoriler.Add(kitapKategori);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Yeni kitap kategorisi başarıyla oluşturuldu: {KategoriAdi}", kitapKategori.Ad);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kategori eklenirken hata oluştu: {KategoriAdi}", kitapKategori.Ad);
                    ModelState.AddModelError("", "Kategori eklenirken bir hata oluştu.");
                }
            }
            else
            {
                _logger.LogWarning("ModelState geçersiz. Hatalar: {Hatalar}", 
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            }

            return View(kitapKategori);
        }

        // GET: KitapKategori/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var kitapKategori = await _context.KitapKategoriler.FindAsync(id);
            if (kitapKategori == null)
            {
                return NotFound();
            }
            return View(kitapKategori);
        }

        // POST: KitapKategori/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad")] KitapKategori kitapKategori)
        {
            if (id != kitapKategori.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kitapKategori);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Kitap kategorisi güncellendi: {KategoriAdi}", kitapKategori.Ad);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitapKategoriExists(kitapKategori.Id))
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
            return View(kitapKategori);
        }

        // GET: KitapKategori/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitapKategori = await _context.KitapKategoriler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (kitapKategori == null)
            {
                return NotFound();
            }

            // Kategoride kitap var mı kontrol et
            var kitapSayisi = await _context.Kitaplar.CountAsync(k => k.KitapKategoriId == id);
            if (kitapSayisi > 0)
            {
                TempData["ErrorMessage"] = "Bu kategoride kitaplar bulunmaktadır. Önce kitapları başka bir kategoriye taşımalısınız.";
                return RedirectToAction(nameof(Index));
            }

            return View(kitapKategori);
        }

        // POST: KitapKategori/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kitapKategori = await _context.KitapKategoriler.FindAsync(id);
            if (kitapKategori != null)
            {
                _context.KitapKategoriler.Remove(kitapKategori);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Kitap kategorisi silindi: {KategoriAdi}", kitapKategori.Ad);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool KitapKategoriExists(int id)
        {
            return _context.KitapKategoriler.Any(e => e.Id == id);
        }
    }
} 