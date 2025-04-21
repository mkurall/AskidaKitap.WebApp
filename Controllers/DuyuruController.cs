using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AskidaKitap.WebApp.Data;
using AskidaKitap.WebApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace AskidaKitap.WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DuyuruController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DuyuruController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Duyuru
        public async Task<IActionResult> Index()
        {
            return View(await _context.Duyurular.OrderByDescending(d => d.OlusturulmaTarihi).ToListAsync());
        }

        // GET: Duyuru/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var duyuru = await _context.Duyurular
                .FirstOrDefaultAsync(m => m.Id == id);
            if (duyuru == null)
            {
                return NotFound();
            }

            return View(duyuru);
        }

        // GET: Duyuru/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Duyuru/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Baslik,Icerik")] Duyuru duyuru)
        {
            if (ModelState.IsValid)
            {
                duyuru.OlusturulmaTarihi = DateTime.Now;
                _context.Add(duyuru);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(duyuru);
        }

        // GET: Duyuru/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var duyuru = await _context.Duyurular.FindAsync(id);
            if (duyuru == null)
            {
                return NotFound();
            }
            return View(duyuru);
        }

        // POST: Duyuru/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Icerik,OlusturulmaTarihi")] Duyuru duyuru)
        {
            if (id != duyuru.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    duyuru.GuncellemeTarihi = DateTime.Now;
                    _context.Update(duyuru);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DuyuruExists(duyuru.Id))
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
            return View(duyuru);
        }

        // GET: Duyuru/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var duyuru = await _context.Duyurular
                .FirstOrDefaultAsync(m => m.Id == id);
            if (duyuru == null)
            {
                return NotFound();
            }

            return View(duyuru);
        }

        // POST: Duyuru/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var duyuru = await _context.Duyurular.FindAsync(id);
            if (duyuru != null)
            {
                _context.Duyurular.Remove(duyuru);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DuyuruExists(int id)
        {
            return _context.Duyurular.Any(e => e.Id == id);
        }
    }
} 