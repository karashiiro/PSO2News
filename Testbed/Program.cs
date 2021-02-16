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
            await foreach (var news in newsTracker.GetNews(after: new DateTime(2021, 1, 1)))
            {
                Console.WriteLine(news.Title);
            }
        }
    }
}
