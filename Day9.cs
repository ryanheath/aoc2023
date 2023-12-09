static partial class Aoc2023
{
    public static void Day9()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                0 3 6 9 12 15
                1 3 6 10 15 21
                10 13 16 21 30 45
                """.ToLines();
            Part1(input).Should().Be(114);
            Part2(input).Should().Be(2);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(1819125966);
            Part2(input).Should().Be(1140);
        }

        int Part1(string[] lines) => lines.ToInts(" ").Select(Predict).Sum();

        int Part2(string[] lines) => lines.ToInts(" ").Select(PredictBackwards).Sum();;

        static int Predict(int[] n)
        {
            var diffs = Differences(n);
            return !diffs.Any() ? 0 : n[^1] + Predict(diffs);
        }

        static int PredictBackwards(int[] n)
        {
            var diffs = Differences(n);
            return !diffs.Any() ? 0 : n[0] - PredictBackwards(diffs);
        }

        static int[] Differences(int[] n) => [..n.Zip(n.Skip(1), (l, r) => r - l)];
    }
}