namespace Discoverify.Tests
{
    using Discoverify.ApiClient;
    using Discoverify.ApiModels;
    using Discoverify.Web.Controllers;
    using Discoverify.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Moq;
    using Moq.Protected;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Xunit;

    public class RecommendationsControllerTest : Controller
    {
        [Fact]
        public async Task Index_Get()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue>();

            var t = new GenreCollection();

            client.Setup(x => x.GetAvailableGenreSeeds()).Returns(Task.FromResult(t));

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Index_Post()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var client = new Mock<IApiClient>();
            var queue = new Mock<IQueue>();

            var r = new RecommendationRepsonse();
            var q = new Queue<RecommendationCollection>();

            client.Setup(x => x.GetRecomendations(It.IsAny<string>())).Returns(Task.FromResult(r));
            queue.Setup(x => x.CreateQueue(It.IsAny<List<RecommendationCollection>>())).Returns(q);

            var controller = new RecommendationsController(client.Object, cache, queue.Object);

            var result = await controller.Index("swedish");

            Assert.IsType<RedirectToActionResult>(result);
        }
    }
}
