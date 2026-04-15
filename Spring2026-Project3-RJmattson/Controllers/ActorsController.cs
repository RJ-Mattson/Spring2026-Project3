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
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public ActorsController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .Include(a => a.ActorMovies)
                .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            var endpoint = new Uri(_configuration["AzureOpenAI:Endpoint"]);
            var key = new System.ClientModel.ApiKeyCredential(_configuration["AzureOpenAI:Key"]);
            ChatClient client = new AzureOpenAIClient(endpoint, key).GetChatClient(_configuration["AzureOpenAI:DeploymentName"]);

            var messages = new ChatMessage[] {
            new SystemChatMessage("You are a Twitter API simulator. Provide 10 short tweets about this actor. Separate each tweet with a '|' character only. No numbers."),
            new UserChatMessage($"Write 10 tweets about the actor {actor.Name}.")
    };

            ClientResult<ChatCompletion> result = await client.CompleteChatAsync(messages);
            string[] tweetTexts = result.Value.Content[0].Text.Split('|', StringSplitOptions.RemoveEmptyEntries);

            var analyzer = new SentimentIntensityAnalyzer();
            var tweetsList = tweetTexts.Select(t => new ViewAITweet
            {
                Tweet = t.Trim(),
                Sentiment = analyzer.PolarityScores(t).Compound
            }).ToList();

            var viewModel = new ViewActor
            {
                Actor = actor,
                Movies = actor.ActorMovies.Select(am => am.Movie).ToList(),
                AITweets = tweetsList,
                AverageSentiment = tweetsList.Any() ? tweetsList.Average(t => t.Sentiment) : 0
            };

            return View(viewModel);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,gender,Age,Imbdlink")] Actor actor, IFormFile PhotoFile)
        {
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await PhotoFile.CopyToAsync(ms);
                    actor.Photo = ms.ToArray();
                }
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,gender,Age,Imbdlink")] Actor actor, IFormFile? PhotoFile)
        {
            if (id != actor.Id)
           {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (PhotoFile != null && PhotoFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await PhotoFile.CopyToAsync(ms);
                            actor.Photo = ms.ToArray();
                        }
                    }
                    else
                    {
                        _context.Entry(actor).Property(a => a.Photo).IsModified = false;
                    }
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
