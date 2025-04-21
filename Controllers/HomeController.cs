using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authorization;

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
            var model = new HomeViewModel();

            // En çok okunan kitapları getir
            var enCokOkunanKitaplar = await _context.KitapKimde
                .GroupBy(k => k.Kitap.Ad)
                .Select(g => new EnCokOkunanKitap
                {
                    KitapAdi = g.Key,
                    OkunmaSayisi = g.Count()
                })
                .OrderByDescending(k => k.OkunmaSayisi)
                .Take(10)
                .ToListAsync();

            // En çok kitap okuyan öğrencileri getir
            var enCokKitapOkuyanOgrenciler = await _context.KitapKimde
                .GroupBy(k => new { k.Ogrenci.Ad, k.Ogrenci.Soyad })
                .Select(g => new EnCokKitapOkuyanOgrenci
                {
                    OgrenciAdi = g.Key.Ad,
                    OgrenciSoyadi = g.Key.Soyad,
                    OkunanKitapSayisi = g.Count()
                })
                .OrderByDescending(o => o.OkunanKitapSayisi)
                .Take(10)
                .ToListAsync();

            // Son duyuruları getir
            var sonDuyurular = await _context.Duyurular
                .OrderByDescending(d => d.OlusturulmaTarihi)
                .Take(5)
                .Select(d => new SonDuyuru
                {
                    Baslik = d.Baslik,
                    Icerik = d.Icerik,
                    OlusturulmaTarihi = d.OlusturulmaTarihi
                })
                .ToListAsync();

            // Son işlemleri getir
            var sonIslemler = await _context.KitapKimde
                .OrderByDescending(k => k.IadeTarihi ?? k.AlinmaTarihi)
                .Take(5)
                .Select(k => new SonIslem
                {
                    IslemTuru = k.IadeTarihi == null ? "Alındı" : "İade",
                    OgrenciAdi = k.Ogrenci.Ad,
                    OgrenciSoyadi = k.Ogrenci.Soyad,
                    KitapAdi = k.Kitap.Ad,
                    IslemTarihi = k.IadeTarihi ?? k.AlinmaTarihi
                })
                .ToListAsync();

            model.EnCokOkunanKitaplar = enCokOkunanKitaplar;
            model.EnCokKitapOkuyanOgrenciler = enCokKitapOkuyanOgrenciler;
            model.SonDuyurular = sonDuyurular;
            model.SonIslemler = sonIslemler;

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Duyurular()
        {
            var duyurular = await _context.Duyurular
                .OrderByDescending(d => d.OlusturulmaTarihi)
                .ToListAsync();
            return View(duyurular);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Islemler()
        {
            var islemler = await _context.KitapKimde
                .OrderByDescending(k => k.IadeTarihi ?? k.AlinmaTarihi)
                .ToListAsync();
            return View(islemler);
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
