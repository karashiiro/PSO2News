using System;
using System.Linq;
using System.Threading.Tasks;
using PSO2News;
using PSO2News.Content;

namespace Testbed
{
    public static class Program
    {
        public static async Task Main()
        {
            var tracker = new PSO2NewsTracker(NewsSource.PSO2es);
            await foreach (var post in tracker.GetNews().Where(n => n.Type == NewsType.Maintenance))
            {
                if (!(post is MaintenanceNewsInfo mni)) continue;
                Console.WriteLine("{0}\n\t{1}\n\t{2}\n\t{3} - {4}", post.Title, post.Type, post.Timestamp, mni.StartTime, mni.EndTime);
            }
        }
    }
}
