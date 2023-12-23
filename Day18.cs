static partial class Aoc2023
{
    public static void Day18()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                R 6 (#70c710)
                D 5 (#0dc571)
                L 2 (#5713f0)
                D 2 (#d2c081)
                R 2 (#59c680)
                D 2 (#411b91)
                L 5 (#8ceee2)
                U 2 (#caa173)
                L 1 (#1b58a2)
                U 2 (#caa171)
                R 2 (#7807d2)
                U 3 (#a77fa3)
                L 2 (#015232)
                U 2 (#7a21e3)
                """.ToLines();
            Part1(input).Should().Be(62);
            Part2(input).Should().Be(952408144115);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(52055);
            Part2(input).Should().Be(67622758357096);
        }

        int Part1(string[] lines) => Fill(Dig(ParseDigPlan(lines, readFromColor: false))).Count;
        long Part2(string[] lines) => Area(ParseDigPlan(lines, readFromColor: true));

        static long Area((Direction dir, int len)[] instructions)
        {
            var pos = (y: 0L, x: 0L);
            var points = new (long y, long x)[instructions.Length];
            var perimeter = 0L;

            for (var i = 0; i < instructions.Length; i++)
            {
                points[i] = pos;
                var instruction = instructions[i];
                var (_, len) = instruction;
                pos = PosFromInstruction(pos, instruction);
                perimeter += len;
            }

            // shoelace formula
            var area = 0L;
            
            for (var i = 0; i < points.Length; i++)
            {
                var nextI = (i + 1) % points.Length;
                var prevI = i - 1 < 0 ? points.Length - 1 : i - 1;
                area += points[i].y * (points[nextI].x - points[prevI].x);
            }

            area = Math.Abs(area) / 2;
            // adjust for the perimeter being the outer edge of the area
            area += perimeter / 2 + 1;

            return area;

            static (long y, long x) PosFromInstruction((long y, long x) pos, (Direction dir, int len) instruction)
            {
                var (dir, len) = instruction;
                var (dy, dx) = dir switch
                {
                    Direction.N => (-len, 0),
                    Direction.S => (+len, 0),
                    Direction.W => (0, -len),
                    Direction.E => (0, +len),
                    _ => throw new UnreachableException()
                };
                    
                return (pos.y + dy, pos.x + dx);
            }
        }

        static HashSet<(int y, int x)> Fill(HashSet<(int y, int x)> digged)
        {
            var minPos = (y: int.MaxValue, x: int.MaxValue);
            var maxPos = (y: int.MinValue, x: int.MinValue);
            foreach (var (y, x) in digged)
            {
                minPos.y = Math.Min(minPos.y, y);
                minPos.x = Math.Min(minPos.x, x);
                maxPos.y = Math.Max(maxPos.y, y);
                maxPos.x = Math.Max(maxPos.x, x);
            }

            if (   Fill((-1, -1), out var filled)
                || Fill((-1, +1), out filled)
                || Fill((+1, +1), out filled)
                || Fill((+1, -1), out filled))
                return filled;

            throw new UnreachableException();

            bool Fill((int y, int x) pos, out HashSet<(int y, int x)> filled)
            {
                filled = [..digged];

                var queue = new Queue<(int y, int x)>();
                queue.Enqueue(pos);

                while (queue.Count > 0)
                {
                    var (y, x) = queue.Dequeue();
                    if (filled.Contains((y, x)))
                        continue;

                    if (y < minPos.y || y > maxPos.y || x < minPos.x || x > maxPos.x)
                        return false;

                    filled.Add((y, x));

                    queue.Enqueue((y - 1, x));
                    queue.Enqueue((y + 1, x));
                    queue.Enqueue((y, x - 1));
                    queue.Enqueue((y, x + 1));
                }

                return true;
            }
        }

        static HashSet<(int y, int x)> Dig((Direction dir, int len)[] instruction)
        {
            var pos = (y: 0, x: 0);
            HashSet<(int y, int x)> digged = [pos];
            
            foreach (var (dir, len) in instruction)
            {
                var (dy, dx) = dir switch
                {
                    Direction.N => (-1, 0),
                    Direction.S => (+1, 0),
                    Direction.W => (0, -1),
                    Direction.E => (0, +1),
                    _ => throw new UnreachableException()
                };
                    
                for (var i = 0; i < len; i++)
                {
                    pos = (pos.y + dy, pos.x + dx);
                    digged.Add(pos);
                }
            }

            return digged;
        }

        static (Direction dir, int len)[] ParseDigPlan(string[] lines, bool readFromColor)
        {
            Func<string, (Direction, int)> parse = readFromColor
                ? ParseInstructionFromColor
                : ParseInstruction;

            return [..lines.Select(parse)];

            static (Direction dir, int len) ParseInstruction(string line)
            {
                var parts = line.Split(' ');
                var dir = parts[0][0] switch
                {
                    'U' => Direction.N,
                    'D' => Direction.S,
                    'L' => Direction.W,
                    'R' => Direction.E,
                    _ => throw new UnreachableException()
                };
                var len = parts[1].ToInt();
                return (dir, len);
            }

            static (Direction dir, int len) ParseInstructionFromColor(string line)
            {
                var parts = line.Split(' ');
                var dir = parts[2][7] switch
                {
                    '3' => Direction.N,
                    '1' => Direction.S,
                    '2' => Direction.W,
                    '0' => Direction.E,
                    _ => throw new UnreachableException()
                };
                var len = parts[2][2..7].ToIntFromHex();
                return (dir, len);
            }
        }
    }
}