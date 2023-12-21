static partial class Aoc2023
{
    public static void Day21()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                ...........
                .....###.#.
                .###.##..#.
                ..#.#...#..
                ....#.#....
                .##..S####.
                .##..#...#.
                .......##..
                .##.#.####.
                .##..##.##.
                ...........
                """.ToLines();
            Part1(input, 6).Should().Be(16);
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input, 64).Should().Be(3574);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines, int steps) => CountPos(steps, ParseMap(lines));
        int Part2(string[] lines) => 0;

        static int CountPos(int steps, (char[][], (int y, int x)) input)
        {
            var (map, start) = input;
            var queue = new Queue<(int y, int x, int s)>();
            queue.Enqueue((start.y, start.x, 0));

            while (queue.Count > 0)
            {
                var (y, x, s) = queue.Dequeue();

                s++;

                if (y - 1 >= 0 && map[y - 1][x] == '.')
                {
                    var p = (y - 1, x, s);
                    if (!queue.Contains(p)) queue.Enqueue(p);
                }
                if (y + 1 < map.Length && map[y + 1][x] == '.')
                {
                    var p = (y + 1, x, s);
                    if (!queue.Contains(p)) queue.Enqueue(p);
                }
                if (x - 1 >= 0 && map[y][x - 1] == '.')
                {
                    var p = (y, x - 1, s);
                    if (!queue.Contains(p)) queue.Enqueue(p);
                }
                if (x + 1 < map[y].Length && map[y][x + 1] == '.')
                {
                    var p = (y, x + 1, s);
                    if (!queue.Contains(p)) queue.Enqueue(p);
                }

                if (queue.All(p => p.s == steps)) break;
            }

            return queue.Count;
        }

        static (char[][], (int, int)) ParseMap(string[] lines)
        {
            var map = new char[lines.Length][];
            var start = (-1, -1);
            for (var y = 0; y < lines.Length; y++)
            {
                map[y] = lines[y].ToCharArray();
                var x = lines[y].IndexOf('S');
                if (x >= 0)
                {
                    start = (y, x);
                    map[y][x] = '.';
                }
            }
            return (map, start);
        }
    }
}