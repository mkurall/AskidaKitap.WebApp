using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;
using System.Diagnostics;
using System.Text;

namespace AskidaKitap.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // En çok okunan kitaplar (Top 10)
            var enCokOkunanKitaplar = await _context.KitapKimde
                .Include(kk => kk.Kitap)
                .GroupBy(kk => new { kk.Kitap.Id, kk.Kitap.Ad, kk.Kitap.Yazar })
                .Select(g => new
                {
                    KitapId = g.Key.Id,
                    KitapAdi = g.Key.Ad,
                    Yazar = g.Key.Yazar,
                    OkunmaSayisi = g.Count()
                })
                .OrderByDescending(x => x.OkunmaSayisi)
                .Take(10)
                .ToListAsync();

            // En çok kitap okuyan öğrenciler (Top 10)
            var enCokKitapOkuyanOgrenciler = await _context.KitapKimde
                .Include(kk => kk.Ogrenci)
                .GroupBy(kk => new { kk.Ogrenci.Id, kk.Ogrenci.Ad, kk.Ogrenci.Soyad })
                .Select(g => new
                {
                    OgrenciId = g.Key.Id,
                    OgrenciAdi = g.Key.Ad,
                    OgrenciSoyadi = g.Key.Soyad,
                    OkunanKitapSayisi = g.Count()
                })
                .OrderByDescending(x => x.OkunanKitapSayisi)
                .Take(10)
                .ToListAsync();

            var model = new HomeViewModel
            {
                EnCokOkunanKitaplar = enCokOkunanKitaplar,
                EnCokKitapOkuyanOgrenciler = enCokKitapOkuyanOgrenciler
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> KitapAra(string aramaKelimesi)
        {
            if (string.IsNullOrWhiteSpace(aramaKelimesi))
            {
                return Json(new { success = false, message = "Lütfen arama yapmak için bir kelime girin." });
            }

            // Türkçe karakterleri normalize et
            var normalizedSearch = NormalizeTurkishCharacters(aramaKelimesi);

            // Önce tüm kitapları al
            var tumKitaplar = await _context.Kitaplar
                .Include(k => k.KitapKategori)
                .Select(k => new
                {
                    k.Id,
                    k.Ad,
                    k.Yazar,
                    Kategori = k.KitapKategori.Ad,
                    StokAdedi = k.StokAdedi,
                    StokDurumu = k.StokAdedi > 0
                })
                .ToListAsync();

            // Sonra bellek üzerinde filtreleme yap
            var kitaplar = tumKitaplar
                .Where(k => 
                    NormalizeTurkishCharacters(k.Ad).Contains(normalizedSearch) || 
                    NormalizeTurkishCharacters(k.Yazar).Contains(normalizedSearch) ||
                    NormalizeTurkishCharacters(k.Kategori).Contains(normalizedSearch))
                .ToList();

            return Json(new { success = true, kitaplar });
        }

        private string NormalizeTurkishCharacters(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalized = text.ToLower();
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                switch (c)
                {
                    case 'ı': sb.Append('i'); break;
                    case 'ğ': sb.Append('g'); break;
                    case 'ü': sb.Append('u'); break;
                    case 'ş': sb.Append('s'); break;
                    case 'ö': sb.Append('o'); break;
                    case 'ç': sb.Append('c'); break;
                    case 'İ': sb.Append('i'); break;
                    case 'Ğ': sb.Append('g'); break;
                    case 'Ü': sb.Append('u'); break;
                    case 'Ş': sb.Append('s'); break;
                    case 'Ö': sb.Append('o'); break;
                    case 'Ç': sb.Append('c'); break;
                    default: sb.Append(c); break;
                }
            }

            return sb.ToString();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> AramaSonuclari(string q)
        {
            if (string.IsNullOrEmpty(q))
            {
                return RedirectToAction("Index");
            }

            // Arama sonuçlarını al
            var tumKitaplar = await _context.Kitaplar
                .Include(k => k.KitapKategori)
                .Select(k => new KitapAramaSonucu
                {
                    Id = k.Id,
                    Ad = k.Ad,
                    Yazar = k.Yazar,
                    Kategori = k.KitapKategori.Ad,
                    StokAdedi = k.StokAdedi,
                    StokDurumu = k.StokAdedi > 0
                })
                .ToListAsync();

            // Türkçe karakterleri normalize et
            var normalizedSearch = NormalizeTurkishCharacters(q);

            // Sonuçları filtrele
            var kitaplar = tumKitaplar
                .Where(k => 
                    NormalizeTurkishCharacters(k.Ad).Contains(normalizedSearch) || 
                    NormalizeTurkishCharacters(k.Yazar).Contains(normalizedSearch) ||
                    NormalizeTurkishCharacters(k.Kategori).Contains(normalizedSearch))
                .ToList();

            ViewBag.AramaKelimesi = q;
            ViewBag.Kitaplar = kitaplar;

            return View();
        }
    }
}
