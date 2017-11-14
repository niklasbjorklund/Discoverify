namespace Discoverify.Web.Services
{
    using Discoverify.ApiModels;
    using System.Collections.Generic;

    public interface IQueue
    {
        Queue<RecommendationCollection> CreateQueue(IEnumerable<RecommendationCollection> collection);
        RecommendationCollection ProcessQueue(Queue<RecommendationCollection> queue);
    }
}
