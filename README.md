# PSO2News
PSO2 news scraper.

## Example
```csharp
var tracker = new PSO2NewsTracker(NewsSource.PSO2);
await foreach (var post in tracker.GetNews().Where(n => n.Type == NewsType.Notice))
{
    if (post is ComicNewsInfo cni)
    {
        Console.WriteLine(cni.Title);
    }
}
```
