using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;
using Spring2026_Project3_RJmattson.Data;
using Spring2026_Project3_RJmattson.Models;
using Spring2026_Project3_RJmattson.Models.ViewModels;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VaderSharp2;

namespace Spring2026_Project3_RJmattson.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public MoviesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(a => a.ActorMovies)
                .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            var endpoint = new Uri(_configuration["AzureOpenAI:Endpoint"]);
            var key = new System.ClientModel.ApiKeyCredential(_configuration["AzureOpenAI:Key"]);
            ChatClient client = new AzureOpenAIClient(endpoint, key).GetChatClient(_configuration["AzureOpenAI:DeploymentName"]);

            var messages = new ChatMessage[] {
            new SystemChatMessage("You are a Movie Reviewer simulator. Generate 5 short movie rewiews about this movie. Separate each review with a '|' character only. No numbers."),
            new UserChatMessage($"Write 5 reviews about the movie {movie.Title}.")
    };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] reviewTexts = result.Value.Content[0].Text.Split('|', StringSplitOptions.RemoveEmptyEntries);

            var analyzer = new SentimentIntensityAnalyzer();
            var reviewList = reviewTexts.Select(t => new ViewAIReview
            {
                Review = t.Trim(),
                Sentiment = analyzer.PolarityScores(t).Compound
            }).ToList();

            var viewModel = new ViewMovie
            {
                Movie = movie,
                Actors = movie.ActorMovies.Select(am => am.Actor).ToList(),
                AIReviews = reviewList,
                AverageSentiment = reviewList.Any() ? reviewList.Average(t => t.Sentiment) : 0
            };

            return View(viewModel);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Genre,ReleaseYear,Imbdlink")] Movie movie, IFormFile PosterFile)
        {
            if (PosterFile != null && PosterFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await PosterFile.CopyToAsync(ms);
                    movie.Poster = ms.ToArray();

                }

            }
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ReleaseYear,Imbdlink")] Movie movie, IFormFile PosterFile)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (PosterFile != null && PosterFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await PosterFile.CopyToAsync(ms);
                            movie.Poster = ms.ToArray();
                        }

                    }
                    else
                    {
                        _context.Entry(movie).Property(m => m.Poster).IsModified = false;
                    }
                        _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
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
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
