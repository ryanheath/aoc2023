record struct Set(int Reds, int Greens, int Blues);
record struct Game(int Id, Set[] Rounds);

static partial class Aoc2023
{
    public static void Day2()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
                Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
                Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
                Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
                """.ToLines();
            Part1(input, new (12, 13, 14)).Should().Be(8);
            Part2(input).Should().Be(2286);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input, new (12, 13, 14)).Should().Be(2237);
            Part2(input).Should().Be(66681);
        }

        int Part1(string[] lines, Set set)
        {
            return ParseGames(lines)
                .Where(IsFittingGame)
                .Sum(x => x.Id);

            bool IsFittingGame(Game game) => game.Rounds.All(x => 
                x.Reds <= set.Reds && 
                x.Greens <= set.Greens && 
                x.Blues <= set.Blues);
        }

        int Part2(string[] lines)
        {
            return ParseGames(lines)
                .Select(SmallestSet)
                .Select(x => x.Reds * x.Greens * x.Blues)
                .Sum();

            Set SmallestSet(Game game)
            {
                var set = new Set(0, 0, 0);

                foreach (var round in game.Rounds)
                {
                    set = set with
                    {
                        Reds = Math.Max(set.Reds, round.Reds),
                        Greens = Math.Max(set.Greens, round.Greens),
                        Blues = Math.Max(set.Blues, round.Blues)
                    };
                }

                return set;
            }
        }

        static IEnumerable<Game> ParseGames(string[] lines)
        {
            return lines.Select(ParseGame);

            static Game ParseGame(string line)
            {
                var gameSplit = line.IndexOf(": ");
                var id = line[5..gameSplit].ToInt();
                var rounds = line[(gameSplit + 2)..].Split("; ");

                return new Game(id, [..rounds.Select(ParseSet)]);

                static Set ParseSet(string round) => round.Split(", ").Aggregate(new Set(), AddCube);

                static Set AddCube(Set set, string cube)
                {
                    var cubeSplit = cube.IndexOf(' ');
                    var count = cube[..cubeSplit].ToInt();
                    var color = cube[(cubeSplit + 1)..];

                    return SetColor(set, color, count);
                }

                static Set SetColor(Set set, string color, int count) => color switch
                {
                    "red" => set with { Reds = count },
                    "green" => set with { Greens = count },
                    "blue" => set with { Blues = count },
                    _ => throw new UnreachableException(),
                };
            }
        }
    }
}