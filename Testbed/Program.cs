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
            var tracker = new PSO2NewsTracker(NewsSource.Global);
            await foreach (var post in tracker.GetNews().Where(n => n.Type == NewsType.Maintenance))
            {
                Console.WriteLine("{0}\n\t{1}\n\t{2}", post.Title, post.Type, post.Timestamp);
            }
        }
    }
}
