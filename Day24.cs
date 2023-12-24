
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
            Part2(input).Should().Be(47);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input, 200_000_000_000_000, 400_000_000_000_000).Should().Be(13910);
            Part2(input).Should().Be(618_534_564_836_937);
        }

        int Part1(string[] lines, long min, long max) => CountIntersectionsXY(ParseHailstones(lines), min, max);
        long Part2(string[] lines) => MagicRockPosition(ParseHailstones(lines));

        static long MagicRockPosition(Hailstone[] hailstones)
        {
            var (hs0, hs1, hs2, hs3) = (hailstones[0], hailstones[1], hailstones[^2], hailstones[^1]);

            foreach (var y in SearchSpace())
                foreach (var x in SearchSpace())
                {
                    var i1 = hs0.IsIntersectionXY(hs1, y, x); if (!i1.intersects) continue;
                    var i2 = hs0.IsIntersectionXY(hs2, y, x); if (!i2.intersects) continue;
                    var i3 = hs0.IsIntersectionXY(hs3, y, x); if (!i3.intersects) continue;

                    if ((i1.y, i1.x) != (i2.y, i2.x) || (i1.y, i1.x) != (i3.y, i3.x)) continue;

                    foreach (var z in SearchSpace())
                    {
                        var z1 = hs1.InterpolateZ(i1.t, z);
                        var z2 = hs2.InterpolateZ(i2.t, z);
                        if (z1 != z2) continue;
                        var z3 = hs3.InterpolateZ(i3.t, z);
                        if (z1 != z3) continue;

                        return (long)(i1.x + i1.y + z1);
                    }
                }

            throw new UnreachableException();

            static IEnumerable<int> SearchSpace() => Enumerable.Range(-300, 600);
        }

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
        private Hailstone VOffsetXY(long dvy, long dvx) => new (P, (V.X + dvx, V.Y + dvy, V.Z));

        public bool IntersectsXY(Hailstone other, long min, long max)
        {
            var (intersects, x, y, _) = IntersectsXY(other);

            if (!intersects) return false;

            return x >= min && x <= max && y >= min && y <= max;
        }

        public double InterpolateZ(double t, long dvz) => Round(P.Z + t * (V.Z + dvz));

        private (bool, double x, double y, double t) IntersectsXY(Hailstone other)
        {
            if (V.X == 0) return (false, 0, 0, -1);
            var dydx = V.Y / (decimal)V.X;
            var c = P.Y - dydx * P.X;

            if (other.V.X == 0) return (false, 0, 0, -1);
            var hdydx = other.V.Y / (decimal)other.V.X;
            var hc = other.P.Y - hdydx * other.P.X;

            if (dydx == hdydx) return (false, 0, 0, -1);

            var x = (hc - c) / (dydx - hdydx);
            var t1 = (x - P.X) / V.X;
            var t2 = (x - other.P.X) / other.V.X;

            if (t1 < 0 || t2 < 0) return (false, 0, 0, -1);

            var y = dydx * (x - P.X) + P.Y;

            return (true, Round((double)x), Round((double)y), (double)t1);
        }

        public (bool intersects, double x, double y, double t) IsIntersectionXY(Hailstone other, long dvy, long dvx)
            => other.VOffsetXY(dvy, dvx).IntersectsXY(VOffsetXY(dvy, dvx));

        static private double Round(double d) => Math.Round(d, 3);
    }
}