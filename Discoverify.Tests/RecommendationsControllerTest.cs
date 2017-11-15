namespace Discoverify.Tests
{
    using Discoverify.ApiClient;
    using Discoverify.ApiModels;
    using Discoverify.Web.Controllers;
    using Discoverify.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class RecommendationsControllerTest : Controller
    {
        [Fact]
        public async Task Index_Get_ReturnView()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            var t = new GenreCollection();

            client.Setup(x => x.GetAvailableGenreSeeds()).Returns(Task.FromResult(t));

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_Get_ClientError_ReturnsNoContent()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            client.Setup(x => x.GetAvailableGenreSeeds()).Callback(() =>
            {
                throw new Exception();
            });

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index();

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Index_Post_ReturnsView()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            var r = new RecommendationRepsonse();
            var q = new Queue<RecommendationCollection>();

            client.Setup(x => x.GetRecomendations(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(r));
            queue.Setup(x => x.CreateQueue(It.IsAny<List<RecommendationCollection>>())).Returns(q);

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index("swedish");

            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public async Task Index_Post_WithNoGenre_ReturnsNotFound()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index("");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Index_Post_QueueError_ReturnsNotFound()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            var r = new RecommendationRepsonse();
            
            client.Setup(x => x.GetRecomendations(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(r));

            queue.Setup(x => x.CreateQueue(It.IsAny<List<RecommendationCollection>>())).Callback(() =>
            {
                throw new Exception();
            });

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index("swedish");

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Result_Get_ReturnsView()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            queue.Setup(x => x.ProcessQueue(It.IsAny<Queue<RecommendationCollection>>())).Returns(GetRecommendation());

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = controller.Result();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Result_Get_ProcessQueueError_ReturnsNotFound()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue<RecommendationCollection>>();

            queue.Setup(x => x.ProcessQueue(It.IsAny<Queue<RecommendationCollection>>())).Callback(() =>
            {
                throw new Exception();
            });

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = controller.Result();

            Assert.IsType<NotFoundResult>(result);
        }

        private RecommendationCollection GetRecommendation()
        {
            var artist = new Artist { Name = "TestArtist" };
            var artistList = new List<Artist>();
            artistList.Add(artist);

            return new RecommendationCollection
            {
                Artists = artistList,
                Album = new Album { Name = "TestAlbum" },
                Name = "TestTrackName",
                Uri = "TestUri",
                DurationMs = 1000,
                Popularity = 100
            };
        }
    }
}
