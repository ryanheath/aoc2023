
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
            Part2(input).Should().Be(0);
        }

        int Part1(string[] lines)
        {
            var (instructions, network) = ParseMap(lines);

            var steps = 0;

            var current = "AAA";

            while (current != "ZZZ")
            {
                current = network[current][instructions[steps % instructions.Length]];
                steps++;
            }

            return steps;
        }

        long Part2(string[] lines)
        {
            var (instructions, network) = ParseMap(lines);

            var nodes = network.Keys.Where(k => k[^1] == 'A').ToArray();
            var steps = nodes.Select(Walk).ToArray();

            return steps.Aggregate(Lcm);

            long Walk(string node)
            {
                var steps = 0L;

                while (node[^1] != 'Z')
                {
                    node = network[node][instructions[steps % instructions.Length]];
                    steps++;
                }

                return steps;
            }
        }

        (int[] instructions, Dictionary<string, string[]> network) ParseMap(string[] lines)
        {
            var instructions = lines[0].Select(c => c == 'L' ? 0 : 1).ToArray();

            var network = new Dictionary<string, string[]>();

            for (var i = 2; i < lines.Length; i++)
            {
                var line = lines[i];
                var parts = line.Split(" = ");
                var name = parts[0];
                var nodes = parts[1].Trim('(', ')').Split(", ");
                network[name] = nodes;
            }

            return (instructions, network);
        }
    }

    private static long Lcm(long a, long b)
    {
        return b / Gcd(a, b) * a;
    }

    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }

        return a;
    }
}