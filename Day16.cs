static partial class Aoc2023
{
    public static void Day16()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                .|...\....
                |.-.\.....
                .....|-...
                ........|.
                ..........
                .........\
                ..../.\\..
                .-.-/..|..
                .|....-|.\
                ..//.|....
                """.ToLines();
            Part1(input).Should().Be(46);
            Part2(input).Should().Be(51);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(7884);
            Part2(input).Should().Be(8185);
        }

        int Part1(string[] lines) => CountEnergized(TraverseTiles(ParseMirrors(lines)));
        int Part2(string[] lines) => CountMaxEnergized(ParseMirrors(lines));

        static int CountMaxEnergized(char[][] mirrors)
        {
            var maxY = mirrors.Length;
            var maxX = mirrors[0].Length;
            var max = 0;

            for (var x = 0; x < maxX; x++)
            {
                FindMaxEnergized(0, x, Direction.S);
                FindMaxEnergized(maxY - 1, x, Direction.N);
            }
            for (var y = 0; y < maxY; y++)
            {
                FindMaxEnergized(y, 0, Direction.E);
                FindMaxEnergized(y, maxX - 1, Direction.W);
            }
            return max;

            void FindMaxEnergized(int y, int x, Direction direction)
                => max = Math.Max(max, CountEnergized(TraverseTiles(mirrors, y, x, direction)));
        }

        static List<Direction>[][] TraverseTiles(char[][] mirrors,
            int startY = 0, int startX = 0, Direction startDirection = Direction.E)
        {
            var maxY = mirrors.Length;
            var maxX = mirrors[0].Length;

            var tiles = new List<Direction>[maxY][];
            for (var y = 0; y < maxY; y++)
            {
                tiles[y] = new List<Direction>[maxX];
                for (var x = 0; x < maxX; x++) tiles[y][x] = [];
            }

            var beams = new Queue<(int y, int x, Direction direction)>();
            beams.Enqueue((startY, startX, startDirection));

            while (beams.Count > 0)
            {
                var (y, x, direction) = beams.Dequeue();

                var tile = tiles[y][x];

                // if beam already exists, skip
                if (tile.Contains(direction)) continue;

                // add beam to mirror tile
                tile.Add(direction);

                switch (mirrors[y][x])
                {
                    case '/':
                        direction = direction switch
                        {
                            Direction.N => Direction.E,
                            Direction.E => Direction.N,
                            Direction.S => Direction.W,
                            Direction.W => Direction.S,
                            _ => direction
                        };
                        break;

                    case '\\':
                        direction = direction switch
                        {
                            Direction.N => Direction.W,
                            Direction.E => Direction.S,
                            Direction.S => Direction.E,
                            Direction.W => Direction.N,
                            _ => direction
                        };
                        break;

                    case '-':
                        switch (direction)
                        {
                            case Direction.N:
                            case Direction.S:
                                ContinueBeam(y, x, Direction.W);
                                ContinueBeam(y, x, Direction.E);
                                continue; // skip to next beam
                        }
                        break;

                    case '|':
                        switch (direction)
                        {
                            case Direction.E:
                            case Direction.W:
                                ContinueBeam(y, x, Direction.N);
                                ContinueBeam(y, x, Direction.S);
                                continue; // skip to next beam
                        }
                        break;
                }

                ContinueBeam(y, x, direction);
            }

            return tiles;

            void ContinueBeam(int y, int x, Direction direction)
            {
                switch (direction)
                {
                    case Direction.N: y--; break;
                    case Direction.E: x++; break;
                    case Direction.S: y++; break;
                    case Direction.W: x--; break;
                }

                if (x >= 0 && y >= 0 && x < maxX && y < maxY)
                    beams.Enqueue((y, x, direction));
            }
        }

        static int CountEnergized(List<Direction>[][] tiles) => tiles.Sum(r => r.Count(b => b.Count > 0));

        static char[][] ParseMirrors(string[] lines)
        {
            var mirrors = new char[lines.Length][];
            for (var y = 0; y < lines.Length; y++)
                mirrors[y] = lines[y].ToCharArray();
            return mirrors;
        }
    }
}