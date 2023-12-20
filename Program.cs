Console.WriteLine("Start");
var totalMs = 0L;

typeof(Aoc2023).GetMethods(BindingFlags.Static | BindingFlags.Public)
    .Where(m => m.Name.StartsWith("Day"))
    .Where(m => m.Name != "DayTemplate")
    .OrderByDescending(m => m.Name.Length)
    .ThenByDescending(m => m.Name)
    .ToList()
    .ForEach(m => 
    {
        Console.Write(m.Name.PadLeft(5));
        var timer = Stopwatch.StartNew(); m.Invoke(null, null); timer.Stop();
        Console.WriteLine($" {timer.ElapsedMilliseconds} ms");
        totalMs += timer.ElapsedMilliseconds;
    });

Console.WriteLine($"Total: {totalMs:#,#} ms");
Console.WriteLine("Done!");