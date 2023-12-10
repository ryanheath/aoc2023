static partial class Aoc2023
{
    public static void Day10()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                .....
                .S-7.
                .|.|.
                .L-J.
                .....
                """.ToLines();
            Part1(input).Should().Be(4);

            input =
                """
                ..F7.
                .FJ|.
                SJ.L7
                |F--J
                LJ...
                """.ToLines();
            Part1(input).Should().Be(8);

            input =
                """
                ...........
                .S-------7.
                .|F-----7|.
                .||.....||.
                .||.....||.
                .|L-7.F-J|.
                .|..|.|..|.
                .L--J.L--J.
                ...........
                """.ToLines();
            Part2(input).Should().Be(4);

            input =
                """
                .F----7F7F7F7F-7....
                .|F--7||||||||FJ....
                .||.FJ||||||||L7....
                FJL7L7LJLJ||LJ.L-7..
                L--J.L7...LJS7F-7L7.
                ....F-J..F7FJ|L7L7L7
                ....L7.F7||L7|.L7L7|
                .....|FJLJ|FJ|F7|.LJ
                ....FJL-7.||.||||...
                ....L---J.LJ.LJLJ...
                """.ToLines();
            Part2(input).Should().Be(8);

            input =
                """
                FF7FSF7F7F7F7F7F---7
                L|LJ||||||||||||F--J
                FL-7LJLJ||||||LJL-77
                F--JF--7||LJLJ7F7FJ-
                L---JF-JLJ.||-FJLJJ7
                |F|F-JF---7F7-L7L|7|
                |FFJF7L7F-JF7|JL---7
                7-L-JL7||F7|L7F-7F7|
                L.L7LFJ|||||FJL7||LJ
                L7JLJL-JLJLJL--JLJ.L
                """.ToLines();
            Part2(input).Should().Be(10);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(6838);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => WalkMaze(CreateMaze(lines));

        int Part2(string[] lines) => 0;

        static int WalkMaze(char[][] maze)
        {
            var h = maze.Length;
            var w = maze[0].Length;

            var spos = FindS();
            var (route1, route2) = DetermineStartRoutes();
            maze[spos.y][spos.x] = '*';
            var steps = 1;

            do
            {
                route1 = Move(route1);
                route2 = Move(route2);
                steps++;
            } while (route1 != route2);
            maze[route1.y][route1.x] = '*';

            return steps;

            ((int y, int x), (int y, int x)) DetermineStartRoutes()
            {
                // S is '|'
                maze[spos.y][spos.x] = '|';
                if (CheckDirection(Direction.N, spos, out var p1)
                    && CheckDirection(Direction.S, spos, out var p2))
                    return (p1, p2);

                // S is '-'
                maze[spos.y][spos.x] = '-';
                if (CheckDirection(Direction.E, spos, out p1)
                    && CheckDirection(Direction.W, spos, out p2))
                    return (p1, p2);
                
                // S is 'L'
                maze[spos.y][spos.x] = 'L';
                if (CheckDirection(Direction.N, spos, out p1)
                    && CheckDirection(Direction.E, spos, out p2))
                    return (p1, p2);

                // S is 'J'
                maze[spos.y][spos.x] = 'J';
                if (CheckDirection(Direction.N, spos, out p1)
                    && CheckDirection(Direction.W, spos, out p2))
                    return (p1, p2);
                
                // S is '7'
                maze[spos.y][spos.x] = '7';
                if (CheckDirection(Direction.S, spos, out p1)
                    && CheckDirection(Direction.W, spos, out p2))
                    return (p1, p2);
                
                // S is 'F'
                maze[spos.y][spos.x] = 'F';
                if (CheckDirection(Direction.S, spos, out p1)
                    && CheckDirection(Direction.E, spos, out p2))
                    return (p1, p2);
                
                throw new UnreachableException("S not determined");
            }

            (int, int) Move((int y, int x) pos)
            {
                for (var d = Direction.N; d <= Direction.W; d++)
                {
                    if (CheckDirection(d, pos, out var newPos))
                    {
                        maze[pos.y][pos.x] = '*';
                        return newPos;
                    }
                }

                throw new UnreachableException("No direction found");
            }

            bool CheckDirection(Direction direction, (int y, int x) pos, out (int y, int x) newPos)
            {
                newPos = maze[pos.y][pos.x] switch
                {
                    '|' or 'L' or 'J' when direction is Direction.N && pos.y > 0 => (pos.y - 1, pos.x),
                    '|' or '7' or 'F' when direction is Direction.S && pos.y < (h - 1) => (pos.y + 1, pos.x),
                    '-' or '7' or 'J' when direction is Direction.W && pos.x > 0 => (pos.y, pos.x - 1),
                    '-' or 'L' or 'F' when direction is Direction.E && pos.x < (w - 1) => (pos.y, pos.x + 1),
                    _ => pos
                };

                if (newPos == pos) return false;

                return (maze[newPos.y][newPos.x], direction) switch
                {
                    ('|', Direction.N) or ('|', Direction.S) or
                    ('-', Direction.E) or ('-', Direction.W) or
                    ('L', Direction.S) or ('L', Direction.W) or
                    ('J', Direction.S) or ('J', Direction.E) or
                    ('7', Direction.N) or ('7', Direction.E) or
                    ('F', Direction.N) or ('F', Direction.W) => true,
                    _ => false
                };
            }

            (int y, int x) FindS()
            {
                for (var y = 0; y < maze.Length; y++)
                    for (var x = 0; x < maze[y].Length; x++)
                        if (maze[y][x] == 'S') return (y, x);
                throw new UnreachableException("S not found");
            }
        }
    }

    enum Direction
    {
        None,
        N,
        E,
        S,
        W
    }

    static char[][] CreateMaze(string[] lines)
    {
        var maze = new char[lines.Length][];
        for (var y = 0; y < lines.Length; y++)
            maze[y] = [.. lines[y].ToArray()];
        return maze;
    }
}