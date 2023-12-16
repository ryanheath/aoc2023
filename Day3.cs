static partial class Aoc2023
{
    public static void Day3()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

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
            var parts = new Dictionary<(int, int), int>();

            foreach (var (_, digitPos, partNumber) in FindPartPositions(lines, @"[^\.0-9]"))
                parts[digitPos] = partNumber;

            return parts.Values;
        }

        IEnumerable<int> ParseGearRatios(string[] lines)
        {
            var gears = new Dictionary<(int, int), HashSet<((int, int), int partNumber)>>();

            foreach (var (symbolPos, digitPos, partNumber) in FindPartPositions(lines, @"\*"))
            {
                gears.TryAdd(symbolPos, []);
                gears[symbolPos].Add((digitPos, partNumber));
            }

            return gears
                    .Where(kv => kv.Value.Count == 2)
                    .Select(kv => kv.Value.First().partNumber * kv.Value.Last().partNumber);
        }

        static IEnumerable<((int, int) symbolPos, (int, int) digitPos, int partNumber)> FindPartPositions(string[] lines, string regEx)
        {
            var symbolsRegex = new Regex(regEx);

            for (var i = 0; i < lines.Length; i++)
            foreach (Match m in symbolsRegex.Matches(lines[i]))
            for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                var x = m.Index + dx;
                var y = i + dy;

                if (y < 0 || y >= lines.Length || x < 0 || x >= lines[y].Length) continue;

                if (char.IsDigit(lines[y][x]))
                {
                    var (digitX, partNumber) = ParsePartNumber(x, lines[y]);
                    yield return ((i, m.Index), (y, digitX), partNumber);
                }
            }
        }

        static (int digitX, int partNumber) ParsePartNumber(int x, string line)
        {
            var startX = x;
            while (startX > 0 && char.IsDigit(line[startX-1])) startX--;

            var endX = x;
            while (endX < line.Length-1 && char.IsDigit(line[endX+1])) endX++;

            var partNumber = line[startX..(endX+1)].ToInt();

            return (startX, partNumber);
        }
    }
}