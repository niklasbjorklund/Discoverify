namespace Discoverify.Web.Services
{
    using System.Collections.Generic;
    using Discoverify.ApiModels;

    public class RecommendationQueue<T> : IQueue<T> where T : RecommendationCollection
    {
        public Queue<T> CreateQueue(IEnumerable<T> collection)
        {
            var queue = new Queue<T>();

            foreach (var item in collection)
            {
                queue.Enqueue(item);
            }

            return queue;
        }

        public T ProcessQueue(Queue<T> queue)
        {
            if (queue.Count > 0)
            {
                return queue.Dequeue();
            }
            return null;
        }
    }
}
