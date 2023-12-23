static partial class Aoc2023
{
    public static void Day13()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

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

        int Part1(string[] lines) => ParseMirrors(lines).Select(x => DetectMirror(x, useSmudge: false)).Sum();
        int Part2(string[] lines) => ParseMirrors(lines).Select(x => DetectMirror(x, useSmudge: true)).Sum();

        static int DetectMirror(string[] mirror, bool useSmudge)
        {
            var mid = IsHorizontalMirror();
            if (mid >= 0) return (mid + 1) * 100;

            return IsVerticalMirror() + 1;

            int IsHorizontalMirror() =>
                ScanMirror(mirror.Length, mirror[0].Length, 
                    (i, i2) => Enumerable.Range(0, mirror[0].Length).Count(x => mirror[i][x] == mirror[i2][x]));

            int IsVerticalMirror() => 
                ScanMirror(mirror[0].Length, mirror.Length, 
                    (i, i2) => mirror.Count(l => l[i] == l[i2]));

            int ScanMirror(int imax, int dmax, Func<int, int, int> getSameCount)
            {
                for (var i = 0; i < imax - 1; i++)
                    if (IsMirror(i, imax, dmax, getSameCount))
                        return i;
                return -1;
            }

            bool IsMirror(int i, int imax, int length, Func<int, int, int> getSameCount)
            {
                var smudgedLength = useSmudge ? length - 1 : length;
                var wasSmudged = false;

                for (var i2 = i + 1; ; i--, i2++)
                {
                    var sameCount = getSameCount(i, i2);

                    if (sameCount != length && sameCount != smudgedLength)
                        break;

                    if (useSmudge && smudgedLength == sameCount)
                    {
                        // smudged may be used only once
                        if (wasSmudged)
                            return false;

                        wasSmudged = true;
                    }

                    // reached one of the ends?
                    if (i == 0 || i2 == imax - 1)
                        return !useSmudge || wasSmudged;
                }

                return false;
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