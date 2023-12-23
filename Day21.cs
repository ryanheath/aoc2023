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
            Part2(input, 6).Should().Be(16);
            Part2(input, 10).Should().Be(50);
            Part2(input, 50).Should().Be(1594);
            Part2(input, 100).Should().Be(6536);
            Part2(input, 500).Should().Be(167004);
            Part2(input, 1000).Should().Be(668697);
            Part2(input, 5000).Should().Be(16733044);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input, 64).Should().Be(3574);
            Part2(input, 26501365).Should().Be(600090522932119);
        }

        long Part1(string[] lines, int steps) => Positions(steps, infinite: false, ParseMap(lines)).Count;
        long Part2(string[] lines, int steps) => CountPositions(steps, ParseMap(lines));

        static PointsHashSet Positions(int steps, bool infinite, (char[][], (int y, int x)) input)
        {
            var (map, start) = input;
            var readQueue = new Queue<(int y, int x, int s)>();
            var writeQueue = new Queue<(int y, int x, int s)>();
            var mapHeight = map.Length;
            var mapWidth = map[0].Length;
            var hasSeen = new PointsHashSet(mapWidth);

            readQueue.Enqueue((start.y, start.x, 0));

            while (readQueue.Count > 0)
            {
                var (y, x, s) = readQueue.Dequeue();

                s++;

                Enqueue(y - 1, x + 0, s);
                Enqueue(y + 1, x + 0, s);
                Enqueue(y + 0, x - 1, s);
                Enqueue(y + 0, x + 1, s);

                if (readQueue.Count == 0)
                {
                    // swap queues
                    hasSeen.Clear();
                    (readQueue, writeQueue) = (writeQueue, readQueue);
                    if (s == steps) break;
                }
            }

            return new PointsHashSet(mapWidth, readQueue.Select(p => (p.y, p.x)));

            void Enqueue(int y, int x, int s)
            {
                if (!IsOpen(y, x)) return;
                var p = (y, x, s);
                var ps = (y, x);
                if (hasSeen.Add(ps))
                {
                    writeQueue.Enqueue(p);
                }
            }

            bool IsOpen(int y, int x)
                => infinite
                    ? map[MathExtensions.PositiveMod(y, mapHeight)][MathExtensions.PositiveMod(x, mapWidth)] == '.'
                    : y >= 0 && y < mapHeight && x >= 0 && x < mapWidth && map[y][x] == '.';
        }

        static long CountPositions(int steps, (char[][], (int y, int x)) input)
        {
            var (map, _) = input;
            var mapSize = map.Length;

            // patterns will emerge when the map is repeated three times in a cross pattern
            //
            //   ...  3x3  ...
            //   3x3  3x3  3x3
            //   ...  3x3  ...
            //
            // the middle will alternate between 2 values
            // the outers will repeat after mapSize steps
            // we need at least 4 * mapSize steps to see a pattern
            // less than 4 * mapSize steps will be brute forced

            var minSteps = 4 * mapSize;

            if (steps < minSteps) return Positions(steps, infinite: true, input).Count;

            // which step between minStep and maxStep will be equivalent to steps?
            var simSteps = (steps - minSteps) % mapSize + minSteps;

            var positions = Positions(simSteps, infinite: true, input);
            var simMap = CompressedMap(simSteps, mapSize, positions);
            DrawCompressedMap(simMap);

            // if (steps < 1000)
            // {
            //     positions = Positions(steps, infinite: true, input);
            //     var cMap = CompressedMap(steps, mapSize, positions);
            //     DrawCompressedMap(steps, cMap);
            // }

            var simTopCorners = CalculateTopCorners(simMap);
            var simCenter = CalculateCenter(simMap, steps, mapSize);
            var simSides = CalculateSides(simMap, steps, mapSize);

            return simCenter + simTopCorners + simSides;

            static long CalculateSides(int[][] simMap, int steps, int mapSize)
            {
                // repeats are the same for all sides
                var orgGrid = steps / mapSize + 1;
                var cSize = orgGrid * 2 + 1;
                var cGrid = cSize / 2;

                var r1 = Math.Max(cGrid - 3, 0L);
                var r2 = Math.Max(cGrid - 4, 0L);
                var r3 = Math.Max(cGrid - 5, 0L);

                var dGrid = simMap.Length / 2;

                // get the three top values of the top corners
                // row 0 is always empty
                var n1 = simMap[1][dGrid+1];
                var n2 = simMap[2][dGrid+1];
                var n3 = simMap[3][dGrid+1];

                // get the three right values of the top corners
                // last col is always empty
                var e1 = simMap[dGrid+1][^2];
                var e2 = simMap[dGrid+1][^3];
                var e3 = simMap[dGrid+1][^4];

                // get the three bottom values of the top corners
                // last row is always empty
                var s1 = simMap[^2][dGrid-1];
                var s2 = simMap[^3][dGrid-1];
                var s3 = simMap[^4][dGrid-1];

                // get the three left values of the top corners
                // col 0 is always empty
                var w1 = simMap[dGrid-1][1];
                var w2 = simMap[dGrid-1][2];
                var w3 = simMap[dGrid-1][3];

                return
                    (n1 + e1 + s1 + w1) * r1 +
                    (n2 + e2 + s2 + w2) * r2 +
                    (n3 + e3 + s3 + w3) * r3;
            }

            static long CalculateCenter(int[][] simMap, int steps, int mapSize)
            {
                // center alternate between 2 values
                var dGrid = simMap.Length / 2;
                var highValue = simMap[dGrid][dGrid];
                var lowValue = simMap[dGrid][dGrid+1];

                var orgGrid = steps / mapSize + 1;
                var cSize = orgGrid * 2 + 1;
                var cGrid = cSize / 2L;
                // adjust for the top corners
                cGrid -= 3;

                // 42 * 1 + 39 * 0
                // 42 * 2 + 39 * 1
                // 42 * 3 + 39 * 2
                // 42 * 4 + 39 * 3
                // 42 * 3 + 39 * 2
                // 42 * 2 + 39 * 1
                // 42 * 1 + 39 * 0
                //
                // => 42 * (1 + 2 + 3 + 4 + 3 + 2 + 1) + 39 * (0 + 1 + 2 + 3 + 2 + 1 + 0)
                // => 42 * (4 + (3 * (3 + 1) / 2) * 2) + 39 * (3 + (2 * (2 + 1) / 2) * 2)
                // => 42 * (4 + 3 * 4) + 39 * (3 + 2 * 3)
                // => 42 * 4*4 + 39 * 3*3
                var highCount = (cGrid + 1) * (cGrid + 1);
                var lowCount = cGrid * cGrid;

                return highCount * highValue + lowCount * lowValue;
            }

            static long CalculateTopCorners(int[][] cMap)
            {
                var dGrid = cMap.Length / 2;
                var count = 0L;

                for (var i = -dGrid; i <= dGrid; i++)
                    for (var j = -dGrid; j <= dGrid; j++)
                    {
                        var ai = Math.Abs(i);
                        var aj = Math.Abs(j);

                        if (IsTopCorner(dGrid, ai, aj))
                            count += cMap[i + dGrid][j + dGrid];
                    }

                return count;
            }

            static int[][] CompressedMap(int steps, int mapSize, PointsHashSet positions)
            {
                var dGrid = steps / mapSize + 1;
                var cSize = dGrid * 2 + 1;

                var compressedMap = new int[cSize][];
                for (var i = 0; i < cSize; i++)
                    compressedMap[i] = new int[cSize];

                for (var i = -dGrid; i <= dGrid; i++)
                    for (var j = -dGrid; j <= dGrid; j++)
                    {
                        var inMapCount = 0;
                        for (var y = 0; y < mapSize; y++)
                            for (var x = 0; x < mapSize; x++)
                                if (positions.Contains((y+i*mapSize, x+j*mapSize)))
                                    inMapCount++;

                        compressedMap[i+dGrid][j+dGrid] = inMapCount;
                    }

                return compressedMap;
            }

            static void DrawCompressedMap(int[][] cMap)
            {
                return;
                var dGrid = cMap.Length / 2;

                Console.WriteLine($"dGrid = {dGrid}");

                for (var i = -dGrid; i <= dGrid; i++)
                {
                    for (var j = -dGrid; j <= dGrid; j++)
                    {
                        var ai = Math.Abs(i);
                        var aj = Math.Abs(j);

                        var oldColor = Console.ForegroundColor;

                        if (IsCenter(dGrid, ai, aj))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else if (IsTopCorner(dGrid, ai, aj))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        }
                        else if (IsSide(dGrid, ai, aj))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Console.Write($"{cMap[i + dGrid][j + dGrid]:00} ");

                        Console.ForegroundColor = oldColor;
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            static bool IsCenter(int dGrid, int ai, int aj) => ai + aj < dGrid - 2;

            static bool IsTopCorner(int dGrid, int ai, int aj) =>
                !IsCenter(dGrid, ai, aj) &&
                (
                    ai == dGrid - 1 && aj == 0 ||
                    ai == dGrid - 1 && aj == 1 ||
                    ai == dGrid - 2 && aj == 0 ||
                    ai == dGrid - 2 && aj == 1 ||
                    ai == dGrid - 3 && aj == 0 ||
                    ai == dGrid - 3 && aj == 1 ||
                    ai == 0 && aj == dGrid - 1 ||
                    ai == 1 && aj == dGrid - 1 ||
                    ai == 0 && aj == dGrid - 2 ||
                    ai == 1 && aj == dGrid - 2 ||
                    ai == 0 && aj == dGrid - 3 ||
                    ai == 1 && aj == dGrid - 3
                );

            static bool IsSide(int dGrid, int ai, int aj) =>
                !IsTopCorner(dGrid, ai, aj) &&
                ai + aj <= dGrid && ai != 0 && aj != 0;
        }

        static (char[][], (int, int)) ParseMap(string[] lines)
        {
            var map = new char[lines.Length][];
            var start = (-1, -1);
            for (var y = 0; y < lines.Length; y++)
            {
                var line = lines[y];
                map[y] = line.ToCharArray();
                var x = line.IndexOf('S');
                if (x >= 0)
                {
                    start = (y, x);
                    map[y][x] = '.';
                }
            }
            return (map, start);
        }
    }
    
    /// specialized hashset for points, to speed up the search
    class PointsHashSet(int gridSize)
    {
        readonly Dictionary<int, HashSet<int>> hashSets = [];

        public PointsHashSet(int gridSize, IEnumerable<(int, int)> enumerable) : this(gridSize)
        {
            foreach (var item in enumerable) Add(item);
        }

        public int Count => hashSets.Values.Sum(hs => hs.Count);

        public bool Add((int y, int x) p) => GetHashSetFor(p).Add(Point(p.y, p.x));

        public void Clear() { foreach (var hashSet in hashSets.Values) hashSet.Clear(); }

        public bool Contains((int y, int x) p) => GetHashSetFor(p).Contains(Point(p.y, p.x));

        private HashSet<int> GetHashSetFor((int y, int x) p)
        {
            var key = GridKey(p.y, p.x);
            if (!hashSets.TryGetValue(key, out var hashSet)) 
            {
                hashSet = new HashSet<int>(gridSize * gridSize);
                hashSets.Add(key, hashSet);
            }
            return hashSet;
        }

        int GridKey(int y, int x) => y / gridSize * 100000 + x / gridSize;
        static int Point(int y, int x) => y * 100000 + x;
    }
}