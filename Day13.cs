static partial class Aoc2023
{
    public static void Day13()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                #.##..##.
                ..#.##.#.
                ##......#
                ##......#
                ..#.##.#.
                ..##..##.
                #.#.##.#.

                #...##..#
                #....#..#
                ..##..###
                #####.##.
                #####.##.
                ..##..###
                #....#..#
                """.ToLines();
            Part1(input).Should().Be(405);
            Part2(input).Should().Be(400);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(30535);
            Part2(input).Should().Be(30844);
        }

        int Part1(string[] lines) => ParseMirrors(lines).Select(DetectMirror).Sum();

        int Part2(string[] lines) => ParseMirrors(lines).Select(DetectSmudgedMirror).Sum();

        static int DetectMirror(string[] mirror)
        {
            // horizontal mirror?
            var mid = IsHorizontalMirror(mirror);
            if (mid >= 0) return (mid + 1) * 100;

            mid = IsVerticalMirror(mirror);
            if (mid >= 0) return mid + 1;

            throw new UnreachableException("no mirror detected");

            static int IsHorizontalMirror(string[] mirror)
            {
                for (var y = 0; y < mirror.Length - 1; y++)
                    if (IsMirror(y))
                        return y;

                return -1;

                bool IsMirror(int y)
                {
                    for (var y2 = y + 1; Enumerable.Range(0, mirror[y].Length).All(x => mirror[y][x] == mirror[y2][x]); y--, y2++)
                        // reached one of the ends?
                        if (y == 0 || y2 == mirror.Length - 1)
                            return true;

                    return false;
                }
            }

            static int IsVerticalMirror(string[] mirror, bool debug = false)
            {
                for (var x = 0; x < mirror[0].Length - 1; x++)
                    if (IsMirror(x))
                        return x;

                return -1;

                bool IsMirror(int x)
                {
                    for (var x2 = x + 1; mirror.All(l => l[x] == l[x2]); x--, x2++)
                        // reached one of the ends?
                        if (x == 0 || x2 == mirror[0].Length - 1)
                            return true;

                    return false;
                }
            }
        }

        static int DetectSmudgedMirror(string[] mirror)
        {
            // horizontal mirror?
            var mid = IsHorizontalMirror(mirror);
            if (mid >= 0) return (mid + 1) * 100;

            mid = IsVerticalMirror(mirror);
            if (mid >= 0) return mid + 1;

            throw new UnreachableException("no mirror detected");

            static int IsHorizontalMirror(string[] mirror)
            {
                for (var y = 0; y < mirror.Length - 1; y++)
                    if (IsMirror(y))
                        return y;

                return -1;

                bool IsMirror(int y)
                {
                    var length = mirror[0].Length;
                    var smudgedLength = length - 1;
                    var wasSmudged = false;

                    for (var y2 = y + 1; ; y--, y2++)
                    {
                        var sameCount = Enumerable.Range(0, mirror[y].Length).Count(x => mirror[y][x] == mirror[y2][x]);

                        if (sameCount != length && sameCount != smudgedLength)
                            break;

                        if (smudgedLength == sameCount)
                        {
                            // smudged may be used only once
                            if (wasSmudged)
                                return false;

                            wasSmudged = true;
                        }

                        // reached one of the ends?
                        if (y == 0 || y2 == mirror.Length - 1)
                            return wasSmudged;
                    }

                    return false;
                }
            }

            static int IsVerticalMirror(string[] mirror, bool debug = false)
            {
                for (var x = 0; x < mirror[0].Length - 1; x++)
                    if (IsMirror(x))
                        return x;

                return -1;

                bool IsMirror(int x)
                {
                    var length = mirror.Length;
                    var smudgedLength = length - 1;
                    var wasSmudged = false;

                    for (var x2 = x + 1; ; x--, x2++)
                    {
                        var sameCount = mirror.Count(l => l[x] == l[x2]);

                        if (sameCount != length && sameCount != smudgedLength)
                            break;

                        if (smudgedLength == sameCount)
                        {
                            // smudged may be used only once
                            if (wasSmudged)
                                return false;

                            wasSmudged = true;
                        }

                        // reached one of the ends?
                        if (x == 0 || x2 == mirror[0].Length - 1)
                            return wasSmudged;
                    }

                    return false;
                }
            }
        }

        static IEnumerable<string[]> ParseMirrors(string[] lines)
        {
            var mirror = new List<string>();

            foreach (var line in lines)
            {
                if (line == "")
                {
                    yield return mirror.ToArray();
                    mirror.Clear();
                }
                else
                {
                    mirror.Add(line);
                }
            }

            if (mirror.Any())
            {
                yield return mirror.ToArray();
            }
        }
    }
}