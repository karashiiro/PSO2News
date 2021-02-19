# PSO2News
![Nuget](https://img.shields.io/nuget/v/PSO2News)

PSO2 news scraper.

Supports the following news sites:
* [PSO2](http://pso2.jp/players/news/)
* [PSO2es](https://pso2.jp/es/players/news/)
* [PSO2 Global](https://pso2.com/news) (No maintenance parsing)
* [PSO2 NGS CBT](https://new-gen.pso2.jp/cbt/players/news/)

## Installation
[`Install-Package PSO2News`](https://www.nuget.org/packages/PSO2News/)

## Example
```csharp
var tracker = new PSO2NewsTracker(NewsSource.PSO2);
await foreach (var post in tracker.GetNews().Where(n => n.Type == NewsType.Announcement))
{
    if (post is ComicNewsInfo cni)
    {
        Console.WriteLine(cni.Title);
    }
}
```
