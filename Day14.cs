static partial class Aoc2023
{
    public static void Day14()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                O....#....
                O.OO#....#
                .....##...
                OO.#O....O
                .O.....O#.
                O.#..O.#.#
                ..O..#O..O
                .......O..
                #....###..
                #OO..#....
                """.ToLines();
            Part1(input).Should().Be(136);
            Part2(input).Should().Be(64);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(109661);
            Part2(input).Should().Be(90176);
        }

        int Part1(string[] lines) => TotalLoadNorth(TiltNorth(ParsePlatform(lines)));

        int Part2(string[] lines) => TotalLoadNorth(TiltCycles(1_000_000_000, ParsePlatform(lines)));

        static int TotalLoadNorth(char[][] platform) 
            => platform.Select((row, y) => row.Count(c => c == 'O') * (platform.Length - y)).Sum();

        static char[][] TiltCycles(int totalCycles, char[][] platform)
        {
            var seen = new Dictionary<long, int>();

            for (var cycle = 1; cycle <= totalCycles; cycle++)
            {
                DoCycle();

                var key = Hash(platform);
                if (seen.TryGetValue(key, out int cycleStart))
                {
                    var cycleLength = cycle - cycleStart;
                    var remainingCycles = (totalCycles - cycleStart) % cycleLength;
                    for (var i = 0; i < remainingCycles; i++) DoCycle();
                    return platform;
                }
                else
                {
                    seen.Add(key, cycle);
                }
            }

            return platform;

            void DoCycle()
            {
                Tilt(platform, Direction.N);
                Tilt(platform, Direction.W);
                Tilt(platform, Direction.S);
                Tilt(platform, Direction.E);
            }
        }

        static long Hash(char[][] platform) 
                => platform.Select((row, y) => row.Select((c, x) => c * (long)x).Sum() * y).Sum();

        static char[][] TiltNorth(char[][] platform) => Tilt(platform, Direction.N);

        static char[][] Tilt(char[][] platform, Direction direction)
        {
            var (dy, dx, ystart, yend, ystep, xstart, xend, xstep) = direction switch
            {
                Direction.N => (-1, +0,                  +1,  platform.Length, +1, 0, platform[0].Length, +1),
                Direction.S => (+1, +0, platform.Length - 2, -1,               -1, 0, platform[0].Length, +1),

                Direction.W => (+0, -1, 0, platform.Length, +1, 1,                       platform[0].Length, +1),
                Direction.E => (+0, +1, 0, platform.Length, +1, platform[0].Length - 2, -1,                  -1),

                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };

            for (var y = ystart; y != yend; y += ystep)
                for (var x = xstart; x != xend; x += xstep)
                {
                    if (platform[y][x] != 'O') continue;

                    var y2 = y + dy;
                    var x2 = x + dx;
                    while (y2 >= 0 && y2 < platform.Length
                        && x2 >= 0 && x2 < platform[0].Length
                        && platform[y2][x2] == '.')
                    {
                        platform[y2][x2] = 'O';
                        platform[y2-dy][x2-dx] = '.';
                        y2 += dy;
                        x2 += dx;
                    }
                }

            return platform;
        }

        static char[][] ParsePlatform(string[] lines)
        {
            var platform = new char[lines.Length][];
            for (var y = 0; y < lines.Length; y++)
                platform[y] = lines[y].ToCharArray();
            return platform;
        }
    }
}