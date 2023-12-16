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

        int Part1(string[] lines) => CountEnergized(TraverseTiles(ParseTiles(lines), lines.Length, lines[0].Length));
        int Part2(string[] lines) => CountMaxEnergized(ParseTiles(lines), lines.Length, lines[0].Length);

        static int CountMaxEnergized(Dictionary<(int y, int x), MirrorTile> tiles, int maxY, int maxX)
        {
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
                => max = Math.Max(max, CountEnergized(TraverseTiles(DeepCopy(tiles), maxY, maxX, y, x, direction)));
        }

        static Dictionary<(int y, int x), MirrorTile> TraverseTiles(Dictionary<(int y, int x), MirrorTile> tiles, 
            int maxY, int maxX,
            int startY = 0, int startX = 0, Direction startDirection = Direction.E)
        {
            var beams = new Queue<(int y, int x, Direction direction)>();
            beams.Enqueue((startY, startX, startDirection));

            while (beams.Count > 0)
            {
                var (y, x, direction) = beams.Dequeue();

                if (!tiles.TryGetValue((y, x), out var tile))
                {
                    // add same direction beam to empty tile
                    tiles[(y, x)] = new MirrorTile('.', [direction]);
                    ContinueBeam(y, x, direction);
                }
                else 
                {
                    // if beam already exists, skip
                    if (tile.Beams.Contains(direction)) continue;

                    // add beam to mirror tile
                    tile.Beams.Add(direction);

                    switch (tile.Mirror)
                    {
                        case '/':
                            var newDirection = direction switch
                            {
                                Direction.N => Direction.E,
                                Direction.E => Direction.N,
                                Direction.S => Direction.W,
                                Direction.W => Direction.S,
                                _ => direction
                            };
                            ContinueBeam(y, x, newDirection);
                            break;

                        case '\\':
                            newDirection = direction switch
                            {
                                Direction.N => Direction.W,
                                Direction.E => Direction.S,
                                Direction.S => Direction.E,
                                Direction.W => Direction.N,
                                _ => direction
                            };
                            ContinueBeam(y, x, newDirection);
                            break;

                        case '-':
                            switch (direction)
                            {
                                case Direction.N:
                                case Direction.S:
                                    ContinueBeam(y, x, Direction.W);
                                    ContinueBeam(y, x, Direction.E);
                                    break;
                                default: ContinueBeam(y, x, direction); break;
                            }
                            break;
                        case '|':
                            switch (direction)
                            {
                                case Direction.E:
                                case Direction.W:
                                    ContinueBeam(y, x, Direction.N);
                                    ContinueBeam(y, x, Direction.S);
                                    break;
                                default: ContinueBeam(y, x, direction); break;
                            }
                            break;
                    
                        default: ContinueBeam(y, x, direction); break;
                    }
                }
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

        static int CountEnergized(Dictionary<(int y, int x), MirrorTile> tiles) 
            => tiles.Values.Count(t => t.Beams.Count > 0);

        static Dictionary<(int y, int x), MirrorTile> ParseTiles(string[] lines)
        {
            var tiles = new Dictionary<(int y, int x), MirrorTile>();
            for (var y = 0; y < lines.Length; y++)
                for (var x = 0; x < lines[y].Length; x++)
                    if (lines[y][x] != '.') tiles[(y, x)] = new MirrorTile(lines[y][x], []);
            return tiles;
        }

        static Dictionary<(int y, int x), MirrorTile> DeepCopy(Dictionary<(int y, int x), MirrorTile> tiles)
        {
            var copy = new Dictionary<(int y, int x), MirrorTile>();
            foreach (var (key, value) in tiles)
                copy[key] = new (value.Mirror, [..value.Beams]);
            return copy;
        }
    }

    record MirrorTile(char Mirror, List<Direction> Beams);
}