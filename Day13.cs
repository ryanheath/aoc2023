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
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(30535);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => ParseMirrors(lines).Select(DetectMirror).Sum();

        int Part2(string[] lines) => 0;

        static int DetectMirror(string[] mirror)
        {
            // horizontal mirror?
            var mid = IsHorizontalMirror(mirror);
            if (mid >= 0) return (mid + 1) * 100;

            mid = IsVerticalMirror(mirror);
            if (mid >= 0) return mid + 1;

            throw new Exception("no mirror detected");

            static int IsHorizontalMirror(string[] mirror)
            {
                for (var y = 0; y < mirror.Length - 1; y++)
                    if (IsMirror(y))
                        return y;

                return -1;

                bool IsMirror(int y)
                {
                    for (var y2 = y + 1; mirror[y] == mirror[y2]; y--, y2++)
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