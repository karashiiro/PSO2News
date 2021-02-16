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
            await foreach (var post in PSO2NewsTracker.GetNews().Where(n => n.Type == NewsType.Maintenance))
            {
                if (!(post is MaintenanceNewsInfo mni)) continue;
                Console.WriteLine("{0}\n{1}: {2}\n\t{3}\n\t{4}", mni.Url, mni.Reason, mni.Title, mni.StartTime.ToShortTimeString(), mni.EndTime.ToShortTimeString());
            }
        }
    }
}
