
static partial class Aoc2023
{
    public static void Day8()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                RL

                AAA = (BBB, CCC)
                BBB = (DDD, EEE)
                CCC = (ZZZ, GGG)
                DDD = (DDD, DDD)
                EEE = (EEE, EEE)
                GGG = (GGG, GGG)
                ZZZ = (ZZZ, ZZZ)
                """.ToLines();
            Part1(input).Should().Be(2);
            input = 
                """
                LLR

                AAA = (BBB, BBB)
                BBB = (AAA, ZZZ)
                ZZZ = (ZZZ, ZZZ)
                """.ToLines();
            Part1(input).Should().Be(6);
            input = 
                """
                LR

                11A = (11B, XXX)
                11B = (XXX, 11Z)
                11Z = (11B, XXX)
                22A = (22B, XXX)
                22B = (22C, 22C)
                22C = (22Z, 22Z)
                22Z = (22B, 22B)
                XXX = (XXX, XXX)
                """.ToLines();
            Part2(input).Should().Be(6);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(13019);
            Part2(input).Should().Be(13524038372771L);
        }

        long Part1(string[] lines) => Walk(ParseMap(lines), "AAA");

        long Part2(string[] lines)
        {
            var map = ParseMap(lines);

            var startingNodes = map.network.Keys.Where(k => k[^1] == 'A');
            var steps = startingNodes.Select(n => Walk(map, n));

            return steps.Lcm();
        }

        static long Walk((int[] instructions, Dictionary<string, string[]> network) map, string node)
        {
            var (instructions, network) = map;
            var steps = 0L;

            while (node[^1] != 'Z')
            {
                node = network[node][instructions[steps % instructions.Length]];
                steps++;
            }

            return steps;
        }

        static (int[] instructions, Dictionary<string, string[]> network) ParseMap(string[] lines)
        {
            int[] instructions = [..lines[0].Select(c => c == 'L' ? 0 : 1)];

            var network = new Dictionary<string, string[]>();

            for (var i = 2; i < lines.Length; i++)
            {
                var parts = lines[i].Split(" = ");
                network[parts[0]] = parts[1].Trim('(', ')').Split(", ");
            }

            return (instructions, network);
        }
    }
}