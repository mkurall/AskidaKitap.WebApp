using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OgrenciController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Ogrenci> _userManager;
        private readonly ILogger<OgrenciController> _logger;

        public OgrenciController(
            ApplicationDbContext context,
            UserManager<Ogrenci> userManager,
            ILogger<OgrenciController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Ogrenci
        public async Task<IActionResult> Index()
        {
            var ogrenciler = await _userManager.Users
                .Include(o => o.Sinif)
                .ToListAsync();
            return View(ogrenciler);
        }

        // GET: Ogrenci/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ogrenci = await _context.Ogrenciler
                .Include(o => o.Kitaplar)
                    .ThenInclude(k => k.Kitap)
                        .ThenInclude(k => k.KitapKategori)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (ogrenci == null)
            {
                return NotFound();
            }

            return View(ogrenci);
        }

        // GET: Ogrenci/Create
        public IActionResult Create()
        {
            ViewBag.Siniflar = new SelectList(_context.Siniflar.Where(s => s.AktifMi), "Id", "Ad");
            return View();
        }

        // POST: Ogrenci/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ogrenci ogrenci, string password)
        {
            if (ModelState.IsValid)
            {
                ogrenci.UserName = ogrenci.Email;
                ogrenci.KayitTarihi = DateTime.Now;
                ogrenci.AktifMi = true;
                ogrenci.OnayDurumu = true;

                var result = await _userManager.CreateAsync(ogrenci, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Yeni öğrenci oluşturuldu: {Email}", ogrenci.Email);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Siniflar = new SelectList(_context.Siniflar.Where(s => s.AktifMi), "Id", "Ad");
            return View(ogrenci);
        }

        // GET: Ogrenci/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ogrenci = await _userManager.FindByIdAsync(id);
            if (ogrenci == null)
            {
                return NotFound();
            }

            ViewBag.Siniflar = new SelectList(_context.Siniflar.Where(s => s.AktifMi), "Id", "Ad");
            return View(ogrenci);
        }

        // POST: Ogrenci/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Ogrenci ogrenci)
        {
            if (id != ogrenci.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingOgrenci = await _userManager.FindByIdAsync(id);
                    if (existingOgrenci == null)
                    {
                        return NotFound();
                    }

                    existingOgrenci.Ad = ogrenci.Ad;
                    existingOgrenci.Soyad = ogrenci.Soyad;
                    existingOgrenci.Email = ogrenci.Email;
                    existingOgrenci.OgrenciNo = ogrenci.OgrenciNo;
                    existingOgrenci.SinifId = ogrenci.SinifId;
                    existingOgrenci.AktifMi = ogrenci.AktifMi;
                    existingOgrenci.OnayDurumu = ogrenci.OnayDurumu;

                    var result = await _userManager.UpdateAsync(existingOgrenci);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Öğrenci güncellendi: {Email}", ogrenci.Email);
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _userManager.Users.AnyAsync(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Siniflar = new SelectList(_context.Siniflar.Where(s => s.AktifMi), "Id", "Ad");
            return View(ogrenci);
        }

        // GET: Ogrenci/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ogrenci = await _userManager.FindByIdAsync(id);
            if (ogrenci == null)
            {
                return NotFound();
            }

            var sinif = await _context.Siniflar.FindAsync(ogrenci.SinifId);
            ViewBag.SinifAdi = sinif?.Ad ?? "Sınıf Yok";

            return View(ogrenci);
        }

        // POST: Ogrenci/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ogrenci = await _userManager.FindByIdAsync(id);
            if (ogrenci != null)
            {
                var result = await _userManager.DeleteAsync(ogrenci);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Öğrenci silindi: {Email}", ogrenci.Email);
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
} 