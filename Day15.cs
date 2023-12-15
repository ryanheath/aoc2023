static partial class Aoc2023
{
    public static void Day15
    ()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            HASH("HASH");

            var input = 
                """
                rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
                """.ToLines();
            Part1(input).Should().Be(1320);
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(510388);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => lines.Select(l => l.Split(',')).SelectMany(l => l).Select(HASH).Sum();
        int Part2(string[] lines) => 0;

        int HASH(string s) => s.Aggregate(0, (h, c) => (h + c) * 17 % 256);
    }
}