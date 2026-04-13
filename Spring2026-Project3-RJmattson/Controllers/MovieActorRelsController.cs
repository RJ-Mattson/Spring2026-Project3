using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_RJmattson.Data;
using Spring2026_Project3_RJmattson.Models;

namespace Spring2026_Project3_RJmattson.Controllers
{
    public class MovieActorRelsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MovieActorRelsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActorMovies.Include(m => m.Actor).Include(m => m.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActorRel = await _context.ActorMovies
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActorRel == null)
            {
                return NotFound();
            }

            return View(movieActorRel);
        }

        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name");
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActorId,MovieId")] MovieActorRel movieActorRel)
        {
            var exists = await _context.ActorMovies.AnyAsync(ma =>
                ma.ActorId == movieActorRel.ActorId && ma.MovieId == movieActorRel.MovieId);

            if (exists)
            {
                ModelState.AddModelError("", "That actor is already assigned to that movie.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(movieActorRel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActorRel = await _context.ActorMovies.FindAsync(id);
            if (movieActorRel == null)
            {
                return NotFound();
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ActorId,MovieId")] MovieActorRel movieActorRel)
        {
            if (id != movieActorRel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movieActorRel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieActorRelExists(movieActorRel.Id))
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
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Name", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Title", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movieActorRel = await _context.ActorMovies
                .Include(m => m.Actor)
                .Include(m => m.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movieActorRel == null)
            {
                return NotFound();
            }

            return View(movieActorRel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movieActorRel = await _context.ActorMovies.FindAsync(id);
            if (movieActorRel != null)
            {
                _context.ActorMovies.Remove(movieActorRel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieActorRelExists(int id)
        {
            return _context.ActorMovies.Any(e => e.Id == id);
        }
    }
}