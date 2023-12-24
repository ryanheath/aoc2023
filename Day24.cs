
static partial class Aoc2023
{
    public static void Day24()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                19, 13, 30 @ -2,  1, -2
                18, 19, 22 @ -1, -1, -2
                20, 25, 34 @ -2, -2, -4
                12, 31, 28 @ -1, -2, -1
                20, 19, 15 @  1, -5, -3
                """.ToLines();
            Part1(input, 7, 27).Should().Be(2);
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input, 200_000_000_000_000, 400_000_000_000_000).Should().Be(13910);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines, long min, long max) => CountIntersectionsXY(ParseHailstones(lines), min, max);
        int Part2(string[] lines) => 0;

        static int CountIntersectionsXY(Hailstone[] hailstones, long min, long max) 
            => hailstones
                .SelectMany((h1, i) => hailstones.Skip(i + 1), (h1, h2) => new { h1, h2 })
                .Count(pair => pair.h1.IntersectsXY(pair.h2, min, max));

        static Hailstone[] ParseHailstones(string[] lines) => [..lines.Select(ParseHailstone)];

        static Hailstone ParseHailstone(string line)
        {
            var parts = line.Split(" @ ");
            var position = parts[0].ToLongs(", ");
            var velocity = parts[1].ToLongs(", ");
            return new Hailstone((position[0], position[1], position[2]), (velocity[0], velocity[1], velocity[2]));
        }
    }

    record Hailstone((long X, long Y, long Z) P, (long X, long Y, long Z) V)
    {
        public bool IntersectsXY(Hailstone other, long min, long max)
        {
            var dydx = V.Y / (double)V.X;
            var c = P.Y - dydx * P.X;

            var hdydx = other.V.Y / (double)other.V.X;
            var hc = other.P.Y - hdydx * other.P.X;

            if (dydx == hdydx) return false;

            var x = (hc - c) / (dydx - hdydx);
            var t1 = (x - P.X) / V.X;
            var t2 = (x - other.P.X) / other.V.X;
            
            if (t1 < 0 || t2 < 0) return false;

            var y = dydx * (x - P.X) + P.Y;

            return x >= min && x <= max && y >= min && y <= max;
        }
    }
}