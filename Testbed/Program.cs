using System;
using System.Threading.Tasks;
using PSO2News;

namespace Testbed
{
    public static class Program
    {
        public static async Task Main()
        {
            var newsTracker = new PSO2NewsTracker();
            await foreach (var news in newsTracker.GetNews())
            {
                Console.WriteLine(news.Title);
            }
        }
    }
}
