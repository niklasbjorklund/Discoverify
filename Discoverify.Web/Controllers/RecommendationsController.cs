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

    public class RecommendationsController : Controller
    {
        private readonly IApiClient _client;
        private Queue<RecommendationCollection> _queue;
        private IMemoryCache _cache;

        public RecommendationsController(IApiClient client, IMemoryCache cache)
        {
            _client = client;
            _cache = cache;
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
            _queue = CreateRecomendationQueue(recomendationsResponse.Tracks);

            _cache.Set("RecomendationQueue", _queue);

            return RedirectToAction("Result");
        }


        [HttpGet]
        public IActionResult Result()
        {
            if (_queue == null)
            {
                _queue = _cache.Get("RecomendationQueue") as Queue<RecommendationCollection>;
            }

            if(_queue.Count > 0)
            { 
                var track = _queue.Dequeue();

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



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ProcessQueue()
        {


            var track = _queue.Dequeue();

            return RedirectToAction("Result", new { track = track });
        }

        private Queue<RecommendationCollection> CreateRecomendationQueue(IEnumerable<RecommendationCollection> collection)
        {
            var queue = new Queue<RecommendationCollection>();

            foreach (var item in collection)
            {
                queue.Enqueue(item);
            }

            return queue;
        }
    }
}
