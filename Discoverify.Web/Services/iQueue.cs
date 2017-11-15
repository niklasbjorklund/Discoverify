namespace Discoverify.Web.Services
{
    using System.Collections.Generic;

    public interface IQueue<T>
    {
        Queue<T> CreateQueue(IEnumerable<T> collection);
        T ProcessQueue(Queue<T> queue);
    }
}
