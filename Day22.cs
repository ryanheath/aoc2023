static partial class Aoc2023
{
    public static void Day22()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                1,0,1~1,2,1
                0,0,2~2,0,2
                0,2,3~2,2,3
                0,0,4~0,2,4
                2,0,5~2,2,5
                0,1,6~2,1,6
                1,1,8~1,1,9
                """.ToLines();
            Part1(input).Should().Be(5);
            Part2(input).Should().Be(7);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(424);
            Part2(input).Should().Be(55483);
        }

        int Part1(string[] lines) => CountDisintegratables(Fall(ParseBricks(lines)));
        int Part2(string[] lines) => CountChainReaction(Fall(ParseBricks(lines)));

        static int CountChainReaction(Brick[] bricks)
        {
            return bricks.Where(IsNotDisintegratable).Sum(ChainReaction);

            int ChainReaction(Brick brick)
            {
                HashSet<Brick> fallen = [brick];
                var work = new Queue<Brick>();
                work.Enqueue(brick);

                while (work.Count > 0)
                {
                    var b = work.Dequeue();

                    foreach (var s in b.Supports)
                    {
                        // skip when still supported by other bricks that stay in place
                        if (s.SupportedBy.Except(fallen).Any()) continue;
                        work.Enqueue(s);
                        fallen.Add(s);
                    }
                }

                return fallen.Count - 1;
            }
        }

        static int CountDisintegratables(Brick[] bricks) => bricks.Count(IsDisintegratable);

        // all supported bricks must be supported by at least one other sibling
        static bool IsDisintegratable(Brick brick) => brick.Supports.All(s => s.SupportedBy.Any(x => x != brick));
        static bool IsNotDisintegratable(Brick brick) => !IsDisintegratable(brick);

        static Brick[] Fall(Brick[] bricks)
        {
            Array.Sort(bricks, (b1, b2) => b1.Z1 - b2.Z1);

            bricks[0].MoveZTo(1);
            var fallen = 1;

            for (var j = 1; j < bricks.Length; j++)
            {
                var brick = bricks[j];

                // find highest Z2 of bricks that have fallen
                // too speed up the fall of the current brick
                var startZ = bricks[..fallen].Max(b => b.Z2);
                brick.MoveZTo(startZ + 1);

                while(brick.Z1 > 1)
                {
                    brick.MoveZTo(brick.Z1 - 1);
                    // fall until brick is supported by another brick
                    if (bricks[..fallen].Any(brick.Intersects))
                    {
                        brick.MoveZTo(brick.Z1 + 1);

                        foreach (var b in bricks[..fallen])
                        {
                            if (brick.IsSupportedBy(b))
                            {
                                brick.SupportedBy.Add(b);
                                b.Supports.Add(brick);
                            }
                        }

                        break;
                    }
                }

                fallen++;
            }

            // set siblings
            for (var i = 0; i < bricks.Length - 1; i++)
                for (var j = i + 1; j < bricks.Length - 1; j++)
                {
                    var b = bricks[j];
                    var brick = bricks[i];

                    if (b.Z2 == brick.Z2)
                    {
                        brick.Siblings.Add(b);
                        b.Siblings.Add(brick);
                    }
                }

            return bricks;
        }

        static Brick[] ParseBricks(string[] lines)
        {
            var bricks = 
                from line in lines
                let coords = line.Split('~')
                select new Brick(coords[0].ToInts(","), coords[1].ToInts(","));
            return [..bricks];
        }
    }

    class Brick(int[] p1, int[] p2)
    {
        public int X1 { get; private set; } = Math.Min(p1[0], p2[0]);
        public int Y1 { get; private set; } = Math.Min(p1[1], p2[1]);
        public int Z1 { get; private set; } = Math.Min(p1[2], p2[2]);
        public int X2 { get; private set; } = Math.Max(p1[0], p2[0]);
        public int Y2 { get; private set; } = Math.Max(p1[1], p2[1]);
        public int Z2 { get; private set; } = Math.Max(p1[2], p2[2]);

        public HashSet<Brick> Supports { get; } = [];
        public HashSet<Brick> SupportedBy { get; } = [];
        public HashSet<Brick> Siblings { get; } = [];

        public void MoveZTo(int z)
        {
            var length = Z2 - Z1;
            Z1 = z;
            Z2 = z + length;
        }

        public bool Intersects(Brick brick) => 
            X1 <= brick.X2 && X2 >= brick.X1 &&
            Y1 <= brick.Y2 && Y2 >= brick.Y1 &&
            Z1 <= brick.Z2 && Z2 >= brick.Z1;

        public bool IsSupportedBy(Brick brick) =>
            X1 <= brick.X2 && X2 >= brick.X1 &&
            Y1 <= brick.Y2 && Y2 >= brick.Y1 &&
            Z1-1 <= brick.Z2 && Z2-1 >= brick.Z1;
    }
}