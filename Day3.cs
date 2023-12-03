static partial class Aoc2023
{
    public static void Day3()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                467..114..
                ...*......
                ..35..633.
                ......#...
                617*......
                .....+.58.
                ..592.....
                ......755.
                ...$.*....
                .664.598..
                """.ToLines();
            Part1(input).Should().Be(4361);
            Part2(input).Should().Be(467835);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(550064);
            Part2(input).Should().Be(85010461);
        }

        int Part1(string[] lines) => ParsePartNumbers(lines).Sum();

        int Part2(string[] lines) => ParseGearRatios(lines).Sum();

        IEnumerable<int> ParsePartNumbers(string[] lines)
        {
            var symbolsRegex = new Regex(@"[^\.0-9]");

            var parts = new HashSet<(int, int, int partNumber)>();

            for (var i = 0; i < lines.Length; i++)
            foreach (Match m in symbolsRegex.Matches(lines[i]))
            for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                var x = m.Index + dx;
                var y = i + dy;

                if (y < 0 || y >= lines.Length || x < 0 || x >= lines[y].Length) continue;

                if (char.IsDigit(lines[y][x])) RegisterPartAt(y, x);
            }

            return parts.Select(p => p.partNumber);

            void RegisterPartAt(int y, int x)
            {
                var line = lines[y];

                var startX = x;
                while (startX > 0 && char.IsDigit(line[startX-1])) startX--;

                var endX = x;
                while (endX < line.Length-1 && char.IsDigit(line[endX+1])) endX++;

                var partNumber = line[startX..(endX+1)].ToInt();

                parts.Add((y, startX, partNumber));
            }
        }

        IEnumerable<int> ParseGearRatios(string[] lines)
        {
            var gearRegex = new Regex(@"\*");

            var gears = new Dictionary<(int, int), HashSet<(int, int, int partNumber)>>();

            for (var i = 0; i < lines.Length; i++)
            foreach (Match m in gearRegex.Matches(lines[i]))
            for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                var x = m.Index + dx;
                var y = i + dy;

                if (y < 0 || y >= lines.Length || x < 0 || x >= lines[y].Length) continue;

                if (char.IsDigit(lines[y][x])) RegisterPartAt(i, m.Index, y, x);
            }

            return gears.Where(kv => kv.Value.Count == 2).Select(kv => kv.Value.First().partNumber * kv.Value.Last().partNumber);

            void RegisterPartAt(int i, int j, int y, int x)
            {
                var line = lines[y];

                var startX = x;
                while (startX > 0 && char.IsDigit(line[startX-1])) startX--;

                var endX = x;
                while (endX < line.Length-1 && char.IsDigit(line[endX+1])) endX++;

                var partNumber = line[startX..(endX+1)].ToInt();

                gears.TryAdd((i, j), []);
                gears[(i, j)].Add((y, startX, partNumber));
            }
        }
    }
}