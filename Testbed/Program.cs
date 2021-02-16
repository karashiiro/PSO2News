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
            await foreach (var post in PSO2NewsTracker.GetNews().Where(n => n.Type == NewsType.Notice))
            {
                if (!(post is ComicNewsInfo cni)) continue;
                Console.WriteLine("{0}\n{1}\n\t{2}", cni.Url, cni.Title, cni.ImageUrl);
            }
        }
    }
}
