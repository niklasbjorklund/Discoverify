namespace Discoverify.Web.Services
{
    using System.Collections.Generic;
    using Discoverify.ApiModels;

    public class RecommendationQueue : IQueue
    {
        public Queue<RecommendationCollection> CreateQueue(IEnumerable<RecommendationCollection> collection)
        {
            var queue = new Queue<RecommendationCollection>();

            foreach (var item in collection)
            {
                queue.Enqueue(item);
            }

            return queue;
        }

        public RecommendationCollection ProcessQueue(Queue<RecommendationCollection> queue)
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }
            return null;
        }
    }
}
