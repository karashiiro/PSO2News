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
            var newsTracker = new PSO2NewsTracker();
            var maintenancePost = await newsTracker.GetNews().FirstOrDefaultAsync(n => n.Type == NewsType.Maintenance);
            if (maintenancePost is MaintenanceNewsInfo mni)
            {
                Console.WriteLine(mni.StartTime.ToShortTimeString());
                Console.WriteLine(mni.EndTime.ToShortTimeString());
                Console.WriteLine(mni.Body);
            }
        }
    }
}
