static partial class Aoc2023
{
    public static void Day10()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

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
                OF----7F7F7F7F-7OOOO
                O|F--7||||||||FJOOOO
                O||OFJ||||||||L7OOOO
                FJL7L7LJLJ||LJIL-7OO
                L--JOL7IIILJS7F-7L7O
                OOOOF-JIIF7FJ|L7L7L7
                OOOOL7IF7||L7|IL7L7|
                OOOOO|FJLJ|FJ|F7|OLJ
                OOOOFJL-7O||O||||OOO
                OOOOL---JOLJOLJLJOOO
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
            Part2(input).Should().Be(451);
        }

        int Part1(string[] lines) => WalkMaze(CreateMaze(lines));

        int Part2(string[] lines)
        {
            var origMaze = CreateMaze(lines); DetermineStartRoutes(origMaze);
            var maze = CreateMaze(lines); WalkMaze(maze);

            // enlarge maze times 3
            var h = maze.Length;
            var w = maze[0].Length;
            var newMaze = new char[h * 2 + 1][];
            for (var y = 0; y < newMaze.Length; y++)
            {
                newMaze[y] = new char[w * 2 + 1];
                Array.Fill(newMaze[y], ' ');
            }
            for (var y = 0; y < h; y++)
                for (var x = 0; x < w; x++)
                {
                    var m = maze[y][x];
                    if (m is '*')
                    {
                        var oc = origMaze[y][x];
                        newMaze[y * 2 + 1][x * 2 + 1] = '*';

                        if (x < w - 1)
                        {
                            if (maze[y][x + 1] is '*') newMaze[y * 2 + 1][x * 2 + 2] = (oc, origMaze[y][x + 1]) switch
                            {
                                ('F', '7') or ('F', 'J') or ('F', '-') or 
                                ('-', '7') or ('-', '-') or ('-', 'J') or 
                                ('L', '-') or ('L', 'J') or ('L', '7') => '*',
                                _ => ' '
                            };
                        }

                        if (y < h - 1)
                        {
                            if (maze[y + 1][x] is '*') newMaze[y * 2 + 2][x * 2 + 1] = (oc, origMaze[y + 1][x]) switch
                            {
                                ('F', 'J') or ('F', 'L') or ('F', '|') or
                                ('7', '|') or ('7', 'L') or ('7', 'J') or 
                                ('|', '|') or ('|', 'L') or ('|', 'J') => '*',
                                _ => ' '
                            };
                        }
                    }
                    else
                    {
                        newMaze[y * 2 + 1][x * 2 + 1] = origMaze[y][x];
                    }
                }

            FloodFill(newMaze);

            // count insides
            var count = 0;

            for (var y = 0; y < newMaze.Length; y++)
                for (var x = 0; x < newMaze[y].Length; x++)
                    if (newMaze[y][x] is not '*' and not '+' and not ' ') count++;

            return count;
        }

        static void FloodFill(char[][] maze)
        {
            var q = new Queue<(int y, int x)>();
            q.Enqueue((0, 0));

            while (q.Count > 0)
            {
                var (y, x) = q.Dequeue();
                if (maze[y][x] is '*' or '+') continue;
                maze[y][x] = '+';
                if (y > 0) q.Enqueue((y - 1, x));
                if (y < maze.Length - 1) q.Enqueue((y + 1, x));
                if (x > 0) q.Enqueue((y, x - 1));
                if (x < maze[0].Length - 1) q.Enqueue((y, x + 1));
            }
        }

        static int WalkMaze(char[][] maze)
        {
            var (spos, route1, route2) = DetermineStartRoutes(maze);
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

            (int, int) Move((int y, int x) pos)
            {
                for (var d = Direction.N; d <= Direction.W; d++)
                {
                    if (CheckDirection(maze, d, pos, out var newPos))
                    {
                        maze[pos.y][pos.x] = '*';
                        return newPos;
                    }
                }

                throw new UnreachableException("No direction found");
            }
        }

        static bool CheckDirection(char[][] maze, Direction direction, (int y, int x) pos, out (int y, int x) newPos)
        {
            var h = maze.Length;
            var w = maze[0].Length;

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

        static ((int y, int x), (int y, int x), (int y, int x)) DetermineStartRoutes(char[][] maze)
        {
            var spos = FindS();
            
            // S is '|'
            maze[spos.y][spos.x] = '|';
            if (CheckDirection(maze, Direction.N, spos, out var p1)
                && CheckDirection(maze, Direction.S, spos, out var p2))
                return (spos, p1, p2);

            // S is '-'
            maze[spos.y][spos.x] = '-';
            if (CheckDirection(maze, Direction.E, spos, out p1)
                && CheckDirection(maze, Direction.W, spos, out p2))
                return (spos, p1, p2);
            
            // S is 'L'
            maze[spos.y][spos.x] = 'L';
            if (CheckDirection(maze, Direction.N, spos, out p1)
                && CheckDirection(maze, Direction.E, spos, out p2))
                return (spos, p1, p2);

            // S is 'J'
            maze[spos.y][spos.x] = 'J';
            if (CheckDirection(maze, Direction.N, spos, out p1)
                && CheckDirection(maze, Direction.W, spos, out p2))
                return (spos, p1, p2);
            
            // S is '7'
            maze[spos.y][spos.x] = '7';
            if (CheckDirection(maze, Direction.S, spos, out p1)
                && CheckDirection(maze, Direction.W, spos, out p2))
                return (spos, p1, p2);
            
            // S is 'F'
            maze[spos.y][spos.x] = 'F';
            if (CheckDirection(maze, Direction.S, spos, out p1)
                && CheckDirection(maze, Direction.E, spos, out p2))
                return (spos, p1, p2);
            
            throw new UnreachableException("S not determined");

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