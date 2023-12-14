static partial class Aoc2023
{
    public static void Day14()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

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
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(109661);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => TotalLoadNorth(TiltNorth(ParsePlatform(lines)));

        int Part2(string[] lines) => 0;

        static int TotalLoadNorth(char[][] platform)
        {
            var load = 0;

            for (var y = 0; y < platform.Length; y++)
                for (var x = 0; x < platform[0].Length; x++)
                    if (platform[y][x] == 'O')
                        load += platform.Length - y;

            return load;
        }

        static char[][] TiltNorth(char[][] platform)
        {
            for (var y = 1; y < platform.Length; y++)
            {
                for (var x = 0; x < platform[0].Length; x++)
                {
                    if (platform[y][x] != 'O') continue;

                    var y2 = y - 1;
                    while (y2 >= 0 && platform[y2][x] == '.')
                    {
                        platform[y2][x] = 'O';
                        platform[y2+1][x] = '.';
                        y2--;
                    }
                }
            }

            return platform;
        }

        static char[][] ParsePlatform(string[] lines)
        {
            var platform = new char[lines.Length][];
            for (var y = 0; y < lines.Length; y++)
            {
                platform[y] = lines[y].ToCharArray();
            }
            return platform;
        }
    }
}