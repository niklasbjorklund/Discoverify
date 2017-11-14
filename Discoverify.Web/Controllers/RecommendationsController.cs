namespace Discoverify.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Discoverify.Web.ViewModels;
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Caching.Memory;
    using Discoverify.ApiModels;
    using Discoverify.ApiClient;
    using Discoverify.Web.Services;

    public class RecommendationsController : Controller
    {
        private readonly IApiClient _client;
        private IMemoryCache _cache;
        private IQueue _queue;

        public RecommendationsController(IApiClient client, IMemoryCache cache, IQueue queue)
        {
            _client = client;
            _cache = cache;
            _queue = queue;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var availableGenres = await _client.GetAvailableGenreSeeds();

            var genres = new RecommendationGenres { Genres = availableGenres.Genres };

            return View(genres);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string genre)
        {
            if (string.IsNullOrEmpty(genre))
            {
                return NotFound();
            }

            var recomendationsResponse = await _client.GetRecomendations(genre);
            var queue = _queue.CreateQueue(recomendationsResponse.Tracks);

            _cache.Set("RecomendationQueue", queue);

            return RedirectToAction("Result");
        }


        [HttpGet]
        public IActionResult Result()
        {
            var cachedQueue = _cache.Get("RecomendationQueue") as Queue<RecommendationCollection>;
            var track = _queue.ProcessQueue(cachedQueue);

            if (track != null)
            { 
                var t = TimeSpan.FromMilliseconds(track.DurationMs);
                var trackLength = $"{(int)t.TotalMinutes}:{t.Seconds:00}";

                var recommendation = new RecommendationResult
                {
                    ArtistName = track.Artists.Select(a => a.Name).FirstOrDefault(),
                    TrackName = track.Name,
                    AlbumName = track.Album.Name,
                    TrackUri = track.Uri,
                    TrackLength = trackLength,
                    TrackPopularity = track.Popularity
                };

                return View(recommendation);
            }

            return RedirectToAction("Index");
        }        

        [HttpPost]
        public IActionResult Next()
        {
            return RedirectToAction("Result");
        }

        [HttpPost]
        public IActionResult Return()
        {
            return RedirectToAction("Index");
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
