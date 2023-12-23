static partial class Aoc2023
{
    public static void Day23()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                #.#####################
                #.......#########...###
                #######.#########.#.###
                ###.....#.>.>.###.#.###
                ###v#####.#v#.###.#.###
                ###.>...#.#.#.....#...#
                ###v###.#.#.#########.#
                ###...#.#.#.......#...#
                #####.#.#.#######.#.###
                #.....#.#.#.......#...#
                #.#####.#.#.#########v#
                #.#...#...#...###...>.#
                #.#.#v#######v###.###v#
                #...#.>.#...>.>.#.###.#
                #####v#.#.###v#.#.###.#
                #.....#...#...#.#.#...#
                #.#########.###.#.#.###
                #...###...#...#...#.###
                ###.###.#.###v#####v###
                #...#...#.#.>.>.#.>.###
                #.###.###.#.###.#.#v###
                #.....###...###...#...#
                #####################.#
                """.ToLines();
            Part1(input).Should().Be(94);
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(2110);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => TraverseLongestPath(ParseMap(lines));
        int Part2(string[] lines) => 0;

        static int TraverseLongestPath(char[][] map)
        {
            int height = map.Length;
            int width = map[0].Length;
            PriorityQueue<(int y, int x, HashSet<(int y, int x)> path), int> queue = new();
            var visited = new Dictionary<(int y, int x), int>();

            queue.Enqueue((0, 1, [(0 , 1)]), 0);

            while (queue.TryDequeue(out var next, out var _))
                Move(next);

            // take lower right corner as target
            // -2 because of the walls
            // -1 because start is not counted
            return visited[(height - 1, width - 2)] - 1;

            void Move((int y, int x, HashSet<(int y, int x)> path) next)
            {
                var (y, x, path) = next;

                var visitedSteps = visited.GetValueOrDefault((y, x));

                if (visitedSteps >= path.Count)
                    return;

                visited[(y, x)] = path.Count;

                switch (map[y][x])
                {
                    case 'v': Enqueue(+1, +0); break;
                    case '>': Enqueue(+0, +1); break;
                    case '<': Enqueue(+0, -1); break;
                    case '^': Enqueue(-1, +0); break;
                    case '.':
                        Enqueue(-1, +0);
                        Enqueue(+0, +1);
                        Enqueue(+1, +0);
                        Enqueue(+0, -1);
                        break;
                }

                void Enqueue(int dy, int dx)
                {
                    var np = (y + dy, x + dx);
                    var (ny, nx) = np;

                    if (path.Contains(np) || ny < 0 || ny >= height || nx < 0 || nx >= width || map[ny][nx] == '#')
                        return;

                    HashSet<(int, int)> nextPath = [np, ..path];

                    queue.Enqueue((ny, nx, nextPath), 1_000_000 - nextPath.Count); // prioritize steps taken
                }
            }
        }

        static char[][] ParseMap(string[] lines)
        {
            var map = new char[lines.Length][];

            for (var y = 0; y < lines.Length; y++)
                map[y] = lines[y].ToCharArray();

            return map;
        }
    }
}
