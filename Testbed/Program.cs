using System;
using System.Linq;
using System.Threading.Tasks;
using PSO2News;

namespace Testbed
{
    public static class Program
    {
        public static async Task Main()
        {
            var tracker = new PSO2NewsTracker(NewsSource.PSO2);
            await foreach (var post in tracker.GetNews().Where(n => n.Type == NewsType.Notice))
            {
                Console.WriteLine(post.Title);
            }
        }
    }
}
