static partial class Aoc2023
{
    public static void Day6()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                Time:      7  15   30
                Distance:  9  40  200
                """.ToLines();
            Part1(input).Should().Be(288);
            Part2(input).Should().Be(71503);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(2344708);
            Part2(input).Should().Be(30125202);
        }

        int Part1(string[] lines) => ParseRaces1(lines).Select(NumberOfWinners).Aggregate(1, (a, r) => a * r);

        int Part2(string[] lines) => NumberOfWinners(ParseRace2(lines));

        int NumberOfWinners((int time, long distance) race)
        {
            var wins = 0;

            for (var i = 0L; i < race.time; i++)
            {
                var distance = (race.time - i) * i;
                if (distance > race.distance)
                {
                    wins++;
                }
            }

            return wins;
        }

        static (int time, long dist)[] ParseRaces1(string[] lines) => [..lines[0][5..].ToInts(" ").Zip(lines[1][9..].ToLongs(" "))];
        static (int time, long dist) ParseRace2(string[] lines) => (lines[0][5..].Replace(" ", "").ToInt(), lines[1][9..].Replace(" ", "").ToLong());
    }
}