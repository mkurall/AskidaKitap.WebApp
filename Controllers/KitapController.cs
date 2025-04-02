using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KitapController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KitapController> _logger;

        public KitapController(ApplicationDbContext context, ILogger<KitapController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Kitap
        public async Task<IActionResult> Index()
        {
            var kitaplar = await _context.Kitaplar
                .Include(k => k.KitapKategori)
                .ToListAsync();

            return View(kitaplar);
        }

        // GET: Kitap/Create
        public async Task<IActionResult> Create()
        {
            var kategoriler = await _context.KitapKategoriler.ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriler, "Id", "Ad");
            return View();
        }

        // POST: Kitap/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad,Yazar,KitapKategoriId,SayfaSayisi,StokAdedi")] Kitap kitap)
        {
            _logger.LogInformation("Create metodu çağrıldı. Gelen veriler: Ad={Ad}, Yazar={Yazar}, KategoriId={KategoriId}, SayfaSayisi={SayfaSayisi}, StokAdedi={StokAdedi}",
                kitap.Ad, kitap.Yazar, kitap.KitapKategoriId, kitap.SayfaSayisi, kitap.StokAdedi);

            // Önce kategoriyi yükle
            var kategori = await _context.KitapKategoriler.FindAsync(kitap.KitapKategoriId);
            if (kategori == null)
            {
                _logger.LogWarning("Seçilen kategori bulunamadı: KategoriId={KategoriId}", kitap.KitapKategoriId);
                ModelState.AddModelError("KitapKategoriId", "Seçilen kategori bulunamadı.");
                await LoadKategoriler();
                return View(kitap);
            }

            // Kategoriyi kitaba ata
            kitap.KitapKategori = kategori;

            // Model validasyonunu kontrol et
            if (!ModelState.IsValid)
            {
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validasyon hatası: {ErrorMessage}", modelError.ErrorMessage);
                }
                await LoadKategoriler();
                return View(kitap);
            }

            try
            {
                _logger.LogInformation("Kitap ekleniyor: Ad={Ad}, Yazar={Yazar}, KategoriId={KategoriId}, SayfaSayisi={SayfaSayisi}, StokAdedi={StokAdedi}",
                    kitap.Ad, kitap.Yazar, kitap.KitapKategoriId, kitap.SayfaSayisi, kitap.StokAdedi);

                _context.Add(kitap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap eklenirken hata oluştu");
                ModelState.AddModelError("", "Kitap eklenirken bir hata oluştu.");
                await LoadKategoriler();
                return View(kitap);
            }
        }

        private async Task LoadKategoriler()
        {
            var kategoriListesi = await _context.KitapKategoriler.ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriListesi, "Id", "Ad");
        }

        // GET: Kitap/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap == null)
            {
                return NotFound();
            }

            var kategoriler = await _context.KitapKategoriler.ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriler, "Id", "Ad");
            return View(kitap);
        }

        // POST: Kitap/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,Yazar,KitapKategoriId,SayfaSayisi,StokAdedi")] Kitap kitap)
        {
            if (id != kitap.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingKitap = await _context.Kitaplar.FindAsync(id);
                    if (existingKitap == null)
                    {
                        return NotFound();
                    }

                    existingKitap.Ad = kitap.Ad;
                    existingKitap.Yazar = kitap.Yazar;
                    existingKitap.KitapKategoriId = kitap.KitapKategoriId;
                    existingKitap.SayfaSayisi = kitap.SayfaSayisi;
                    existingKitap.StokAdedi = kitap.StokAdedi;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KitapExists(kitap.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var kategoriler = await _context.KitapKategoriler.ToListAsync();
            ViewBag.Kategoriler = new SelectList(kategoriler, "Id", "Ad");
            return View(kitap);
        }

        // GET: Kitap/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var kitap = await _context.Kitaplar
                .Include(k => k.KitapKategori)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (kitap == null)
            {
                return NotFound();
            }

            return View(kitap);
        }

        // POST: Kitap/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var kitap = await _context.Kitaplar.FindAsync(id);
            if (kitap != null)
            {
                try
                {
                    _context.Kitaplar.Remove(kitap);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kitap silinirken hata oluştu");
                    TempData["ErrorMessage"] = "Kitap silinirken bir hata oluştu.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KitapExists(int id)
        {
            return _context.Kitaplar.Any(e => e.Id == id);
        }
    }
} 