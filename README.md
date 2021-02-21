[![Nuget](https://img.shields.io/nuget/v/PSO2News)](https://www.nuget.org/packages/PSO2News/)

# PSO2News
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

## Notes
The [regex mess](https://github.com/karashiiro/PSO2News/blob/main/PSO2News/Content/MaintenanceNewsInfo.cs) I'm using for maintenance parsing does not catch every possible formatting case -- it particularly fails on unusual maintenance update announcements. In the event that a post fails to be parsed, the boolean property `Unreadable` will be set to true.
