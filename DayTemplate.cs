static partial class Aoc2023
{
    public static void DayTemplate()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                abd
                """.ToLines();
            Part1(input).Should().Be(0);

            input = 
                """
                abc
                """.ToLines();
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(0);
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines) => 0;

        int Part2(string[] lines) => 0;
    }
}