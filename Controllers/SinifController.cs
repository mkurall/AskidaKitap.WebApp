using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;
using Microsoft.Extensions.Logging;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SinifController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SinifController> _logger;

        public SinifController(ApplicationDbContext context, ILogger<SinifController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Sinif
        public async Task<IActionResult> Index()
        {
            var siniflar = await _context.Siniflar.ToListAsync();
            return View(siniflar);
        }

        // GET: Sinif/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sinif/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Ad")] Sinif sinif)
        {
            if (ModelState.IsValid)
            {
                sinif.KayitTarihi = DateTime.Now;
                sinif.AktifMi = true;
                _context.Add(sinif);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sinif);
        }

        // GET: Sinif/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sinif = await _context.Siniflar.FindAsync(id);
            if (sinif == null)
            {
                return NotFound();
            }
            _logger.LogInformation($"Sınıf düzenleme sayfası açıldı. ID: {id}, Ad: {sinif.Ad}");
            return View(sinif);
        }

        // POST: Sinif/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Ad,AktifMi")] Sinif sinif)
        {
            if (id != sinif.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var mevcutSinif = await _context.Siniflar.FindAsync(id);
                    if (mevcutSinif == null)
                    {
                        return NotFound();
                    }

                    mevcutSinif.Ad = sinif.Ad;
                    mevcutSinif.AktifMi = sinif.AktifMi;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinifExists(sinif.Id))
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
            return View(sinif);
        }

        // GET: Sinif/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sinif = await _context.Siniflar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sinif == null)
            {
                return NotFound();
            }

            // Sınıfta öğrenci var mı kontrol et
            var ogrenciSayisi = await _context.Ogrenciler.CountAsync(o => o.SinifId == id);
            if (ogrenciSayisi > 0)
            {
                TempData["ErrorMessage"] = "Bu sınıfta öğrenciler bulunmaktadır. Önce öğrencileri başka bir sınıfa taşımalısınız.";
                return RedirectToAction(nameof(Index));
            }

            return View(sinif);
        }

        // POST: Sinif/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sinif = await _context.Siniflar.FindAsync(id);
            if (sinif != null)
            {
                _context.Siniflar.Remove(sinif);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Sınıf silindi: {SinifAdi}", sinif.Ad);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SinifExists(int id)
        {
            return _context.Siniflar.Any(e => e.Id == id);
        }
    }
} 