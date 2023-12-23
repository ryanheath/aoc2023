static partial class Aoc2023
{
    public static void Day17()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                2413432311323
                3215453535623
                3255245654254
                3446585845452
                4546657867536
                1438598798454
                4457876987766
                3637877979653
                4654967986887
                4564679986453
                1224686865563
                2546548887735
                4322674655533
                """.ToLines();
            Part1(input).Should().Be(102);
            Part2(input).Should().Be(94);
            input = 
                """
                111111111111
                999999999991
                999999999991
                999999999991
                999999999991
                """.ToLines();
            Part2(input).Should().Be(71);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(767);
            Part2(input).Should().Be(904);
        }

        int Part1(string[] lines) => Traverse(ParseMap(lines), 1, 3);
        int Part2(string[] lines) => Traverse(ParseMap(lines), 4, 10);

        static int Traverse(int[][] map, int minSteps, int maxSteps)
        {
            PriorityQueue<(int y, int x, Direction direction, int directionMoves), int> queue = new();
            var visited = new Dictionary<(Direction, int), int>[map.Length][];
            for (var y = 0; y < map.Length; y++)
            {
                visited[y] = new Dictionary<(Direction, int), int>[map[0].Length];
                for (var x = 0; x < map[0].Length; x++)
                    visited[y][x] = [];
            }

            queue.Enqueue((0, 0, Direction.E, 0), 0);
            queue.Enqueue((0, 0, Direction.S, 0), 0);

            while (queue.Count > 0)
            {
                var (y, x, direction, directionMoves) = queue.Dequeue();

                var heat = visited[y][x].GetValueOrDefault((direction, directionMoves));

                if (directionMoves < maxSteps)
                    Move(y, x, direction, heat, directionMoves);

                if (directionMoves >= minSteps)
                {
                    Move(y, x, L90(direction), heat, 0);
                    Move(y, x, R90(direction), heat, 0);
                }
            }

            var maxY = map.Length - 1;
            var maxX = map[0].Length - 1;

            return visited[maxY][maxX].Min(x => x.Value);

            void Move(int y, int x, Direction direction, int heat, int directionMoves)
            {
                var dy = direction switch
                {
                    Direction.N => -1,
                    Direction.S => 1,
                    _ => 0
                };

                var dx = direction switch
                {
                    Direction.E => 1,
                    Direction.W => -1,
                    _ => 0
                };

                for (var i = 1; i <= maxSteps; i++)
                {
                    var newY = y + i * dy;
                    var newX = x + i * dx;
                    var newDirectionMoves = directionMoves + i;

                    if (newY < 0 || newY >= map.Length || newX < 0 || newX >= map[0].Length || newDirectionMoves > maxSteps)
                        return;

                    heat += map[newY][newX];

                    if (i < minSteps) continue;

                    var vlist = visited[newY][newX];

                    if (vlist.TryGetValue((direction,newDirectionMoves), out var visitedHeat))
                    {
                        if (visitedHeat <= heat)
                            return;
                    }

                    queue.Enqueue((newY, newX, direction, newDirectionMoves), heat);
                    vlist[(direction, newDirectionMoves)] = heat;
                }
            }

            static Direction L90(Direction direction) => direction switch
            {
                Direction.N => Direction.W,
                Direction.W => Direction.S,
                Direction.S => Direction.E,
                Direction.E => Direction.N,
                _ => throw new UnreachableException()
            };
            static Direction R90(Direction direction) => direction switch
            {
                Direction.N => Direction.E,
                Direction.E => Direction.S,
                Direction.S => Direction.W,
                Direction.W => Direction.N,
                _ => throw new UnreachableException()
            };  
        }

        static int[][] ParseMap(string[] lines)
        {
            var map = new int[lines.Length][];

            for (var y = 0; y < lines.Length; y++)
            {
                map[y] = new int[lines[y].Length];
                for (var x = 0; x < lines[y].Length; x++)
                    map[y][x] = lines[y][x] - '0';
            }

            return map;
        }
    }
}