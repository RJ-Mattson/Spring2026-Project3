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

        // GET: MovieActorRels
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ActorMovies.Include(m => m.Actor).Include(m => m.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MovieActorRels/Details/5
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

        // GET: MovieActorRels/Create
        public IActionResult Create()
        {
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Id");
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id");
            return View();
        }

        // POST: MovieActorRels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ActorId,MovieId")] MovieActorRel movieActorRel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movieActorRel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Id", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        // GET: MovieActorRels/Edit/5
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
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Id", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        // POST: MovieActorRels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["ActorId"] = new SelectList(_context.Actors, "Id", "Id", movieActorRel.ActorId);
            ViewData["MovieId"] = new SelectList(_context.Movies, "Id", "Id", movieActorRel.MovieId);
            return View(movieActorRel);
        }

        // GET: MovieActorRels/Delete/5
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

        // POST: MovieActorRels/Delete/5
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
