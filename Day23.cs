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
            Part2(input).Should().Be(154);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(2110);
            Part2(input).Should().Be(6514);
        }

        int Part1(string[] lines) => TraverseLongestPath(ParseMap(lines), slipperySlope: true);
        int Part2(string[] lines) => TraverseLongestPath(ParseMap(lines), slipperySlope: false);

        static int TraverseLongestPath(char[][] map, bool slipperySlope)
        {
            int height = map.Length;
            int width = map[0].Length;

            var corners = FindCorners();

            var pathSegments = FindSegments(corners);

            return FindLongestPath(pathSegments);

            int FindLongestPath(Dictionary<(int cy1, int cx1, int cy2, int cx2), int> pathSegments)
            {
                // find longest path of path segments
                int maxLength = 0;
                Queue<(int cy, int cx, HashSet<(int cy1, int cx1, int cy2, int cx2)> path)> queue = new();
                queue.Enqueue((0, 1, []));

                var exitSegment = pathSegments.Keys.First(x => (x.cy2, x.cx2) == (height - 1, width - 2));

                while (queue.TryDequeue(out var next))
                    Move(next);

                return maxLength;

                void Move((int cy, int cx, HashSet<(int cy1, int cx1, int cy2, int cx2)> path) next)
                {
                    var (cy, cx, path) = next;

                    foreach (var ps in pathSegments.Keys)
                    {
                        if ((cy, cx) != (ps.cy1, ps.cx1) 
                            || path.Contains(ps)
                            || path.Any(p => ps.cy2 == p.cy1 && ps.cx2 == p.cx1))
                            continue;

                        HashSet<(int cy1, int cx1, int cy2, int cx2)> npath = [ps, ..path];

                        if ((ps.cy2, ps.cx2) == (height - 1, width - 2))
                        {
                            // reached exit
                            maxLength = Math.Max(maxLength, npath.Sum(x => pathSegments[x]));
                            continue;
                        }

                        // skip if all possible exits are passed
                        if (npath.Any(p => (p.cy1, p.cx1) == (exitSegment.cy1, exitSegment.cx1)))
                            continue;

                        queue.Enqueue((ps.cy2, ps.cx2, npath));
                    }
                }
            }

            Dictionary<(int cy1, int cx1, int cy2, int cx2), int> FindSegments(HashSet<(int cy, int cx)> corners)
            {
                Dictionary<(int cy1, int cx1, int cy2, int cx2), int> pathSegments = new();

                foreach (var (cy, cx) in corners.Except([(height - 1, width - 2)])) // exit can not be started
                {
                    // find path to other corners
                    Queue<(int y, int x, HashSet<(int y, int x)> path)> work = new();
                    work.Enqueue((cy, cx, [(cy, cx)]));

                    while (work.TryDequeue(out var next))
                    {
                        var (y, x, path) = next;

                        if (slipperySlope)
                            switch (map[y][x])
                            {
                                case 'v': EnqueueWork(+1, +0); continue;
                                case '>': EnqueueWork(+0, +1); continue;
                                case '<': EnqueueWork(+0, -1); continue;
                                case '^': EnqueueWork(-1, +0); continue;
                            }

                        EnqueueWork(+1, +0);
                        EnqueueWork(+0, +1);
                        EnqueueWork(+0, -1);
                        EnqueueWork(-1, +0);

                        void EnqueueWork(int dy, int dx)
                        {
                            var ny = y + dy;
                            var nx = x + dx;
                            var np = (ny, nx);

                            if (path.Contains(np) || ny < 0 || ny >= height || nx < 0 || nx >= width || map[ny][nx] == '#')
                                return;

                            if ((y, x) != np && (0, 1) != np && corners.Contains(np)) // entrance can not be reached
                            {
                                // reached corner, keep longest path
                                var cs = (cy, cx, ny, nx);
                                if (pathSegments.ContainsKey(cs))
                                {
                                    if (pathSegments[cs] < path.Count)
                                        // found shorter path, keep longest
                                        pathSegments[cs] = path.Count;
                                }
                                else
                                    // new segment
                                    pathSegments[cs] = path.Count;

                                return;
                            }

                            work.Enqueue((ny, nx, [np, ..path]));
                        }
                    }
                }

                return pathSegments;
            }

            HashSet<(int cy, int cx)> FindCorners()
            {
                HashSet<(int cy, int cx)> corners = [];

                for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    // ignore entrance and exit
                    if (map[y][x] == '#' || y == 0 && x == 1 || y == height - 1 && x == width - 2) continue;

                    var count = 0;
                    if (map[y - 1][x] != '#' && map[y][x - 1] != '#') count++;
                    if (map[y - 1][x] != '#' && map[y][x + 1] != '#') count++;
                    if (map[y + 1][x] != '#' && map[y][x - 1] != '#') count++;
                    if (map[y + 1][x] != '#' && map[y][x + 1] != '#') count++;

                    if (count > 1) corners.Add((y, x));
                }

                // add entrance and exit
                corners.Add((0, 1));
                corners.Add((height - 1, width - 2));

                return corners;
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
