using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WordRace000.Data;
using WordRace000.Models;

namespace WordRace000.Controllers
{
    [Authorize]
    public class WordController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WordController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var words = await _context.Words
                .Include(w => w.Category)
                .ToListAsync();
            return View(words);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var word = await _context.Words
                .Include(w => w.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (word == null)
            {
                return NotFound();
            }

            return View(word);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Category, "Id", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,English,Turkish,Example,ImageUrl,AudioUrl,Difficulty,CategoryId")] Word word)
        {
            if (ModelState.IsValid)
            {
                _context.Add(word);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Category, "Id", "CategoryName", word.CategoryId);
            return View(word);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var word = await _context.Words.FindAsync(id);
            if (word == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(_context.Category, "Id", "CategoryName", word.CategoryId);
            return View(word);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,English,Turkish,Example,ImageUrl,AudioUrl,Difficulty,CategoryId")] Word word)
        {
            if (id != word.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(word);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WordExists(word.Id))
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
            ViewBag.Categories = new SelectList(_context.Category, "Id", "CategoryName", word.CategoryId);
            return View(word);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var word = await _context.Words
                .Include(w => w.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (word == null)
            {
                return NotFound();
            }

            return View(word);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var word = await _context.Words.FindAsync(id);
            if (word != null)
            {
                _context.Words.Remove(word);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WordExists(int id)
        {
            return _context.Words.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> CheckWordExists([FromBody] string word)
        {
            var exists = await _context.Words.AnyAsync(w => 
                w.English.ToLower() == word.ToLower() || 
                w.Turkish.ToLower() == word.ToLower());
            
            return Json(new { exists });
        }
    }
} 