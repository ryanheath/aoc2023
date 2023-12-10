Console.WriteLine("Start");

typeof(Aoc2023).GetMethods(BindingFlags.Static | BindingFlags.Public)
    .Where(m => m.Name.StartsWith("Day"))
    .Where(m => m.Name != "DayTemplate")
    .OrderByDescending(m => m.Name.Length)
    .ThenByDescending(m => m.Name)
    .ToList()
    .ForEach(m => m.Invoke(null, null));

Console.WriteLine("Done!");