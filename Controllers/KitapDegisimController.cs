using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class KitapDegisimController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<KitapDegisimController> _logger;

        public KitapDegisimController(ApplicationDbContext context, ILogger<KitapDegisimController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: KitapDegisim/OgrenciAra
        public async Task<IActionResult> OgrenciAra(string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return Json(new { success = false, message = "Arama metni boş olamaz." });
            }

            searchString = searchString.Trim().ToLower();

            var ogrenciler = await _context.Users
                .Where(u => u.UserName.ToLower().Contains(searchString) || 
                           u.Ad.ToLower().Contains(searchString) || 
                           u.Soyad.ToLower().Contains(searchString) ||
                           u.OgrenciNo.ToLower().Contains(searchString))
                .Select(u => new
                {
                    u.Id,
                    u.OgrenciNo,
                    u.Ad,
                    u.Soyad
                })
                .ToListAsync();

            return Json(new { success = true, data = ogrenciler });
        }

        // POST: KitapDegisim/KitapVer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KitapVer(int kitapId, string ogrenciId)
        {
            try
            {
                // Kitabın stok durumunu kontrol et
                var kitap = await _context.Kitaplar.FindAsync(kitapId);
                if (kitap == null)
                {
                    return Json(new { success = false, message = "Kitap bulunamadı." });
                }

                if (kitap.StokAdedi <= 0)
                {
                    return Json(new { success = false, message = "Kitap stokta bulunmuyor." });
                }

                // Öğrencinin bu kitabı aktif olarak alıp almadığını kontrol et
                var aktifKayit = await _context.KitapKimde
                    .FirstOrDefaultAsync(k => k.KitapId == kitapId && 
                                            k.OgrenciId == ogrenciId && 
                                            k.IadeTarihi == null);

                if (aktifKayit != null)
                {
                    return Json(new { success = false, message = "Bu öğrenci kitabı zaten almış ve henüz iade etmemiş." });
                }

                // Yeni kayıt oluştur
                var yeniKayit = new KitapKimde
                {
                    KitapId = kitapId,
                    OgrenciId = ogrenciId,
                    AlinmaTarihi = DateTime.Now
                };

                // Stok adedini güncelle
                kitap.StokAdedi--;

                // Değişiklikleri kaydet
                _context.KitapKimde.Add(yeniKayit);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kitap başarıyla verildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap verilirken hata oluştu");
                return Json(new { success = false, message = "Kitap verilirken bir hata oluştu." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KitapIade(int kitapKimdeId)
        {
            try
            {
                var kayit = await _context.KitapKimde
                    .Include(k => k.Kitap)
                    .FirstOrDefaultAsync(k => k.Id == kitapKimdeId && k.IadeTarihi == null);

                if (kayit == null)
                {
                    return Json(new { success = false, message = "Kayıt bulunamadı veya kitap zaten iade edilmiş." });
                }

                // İade tarihini güncelle
                kayit.IadeTarihi = DateTime.Now;

                // Stok adedini artır
                kayit.Kitap.StokAdedi++;

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Kitap başarıyla iade alındı." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kitap iade alınırken hata oluştu");
                return Json(new { success = false, message = "Kitap iade alınırken bir hata oluştu." });
            }
        }
    }
} 