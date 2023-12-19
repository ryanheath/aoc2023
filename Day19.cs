static partial class Aoc2023
{
    public static void Day19()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                px{a<2006:qkq,m>2090:A,rfg}
                pv{a>1716:R,A}
                lnx{m>1548:A,A}
                rfg{s<537:gd,x>2440:R,A}
                qs{s>3448:A,lnx}
                qkq{x<1416:A,crn}
                crn{x>2662:A,R}
                in{s<1351:px,qqz}
                qqz{s>2770:qs,m<1801:hdj,R}
                gd{a>3333:R,R}
                hdj{m>838:A,pv}

                {x=787,m=2655,a=1222,s=2876}
                {x=1679,m=44,a=2067,s=496}
                {x=2036,m=264,a=79,s=2244}
                {x=2461,m=1339,a=466,s=291}
                {x=2127,m=1623,a=2188,s=1013}
                """.ToLines();
            Part1(input).Should().Be(19114);
            Part2(input).Should().Be(0);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(492702);
            Part2(input).Should().Be(167409079868000);
        }

        int Part1(string[] lines) => Sort(ParseRulesAndParts(lines)).accepted.Sum(p => p.TotalValue);
        long Part2(string[] lines) => 0;

        (Part[] accepted, Part[] rejected) Sort((Dictionary<string, Rules>, Part[]) input)
        {
            var accepted = new List<Part>();
            var rejected = new List<Part>();
            var (rules, parts) = input;

            foreach (var part in parts)
                for (var ruleName = rules["in"].NextRule(part); ; ruleName = rules[ruleName].NextRule(part))
                    if (ruleName == "A")
                    {
                        accepted.Add(part);
                        break;
                    }
                    else if (ruleName == "R")
                    {
                        rejected.Add(part);
                        break;
                    }

            return ([..accepted], [..rejected]);
        }

        (Dictionary<string, Rules>, Part[]) ParseRulesAndParts(string[] lines)
        {
            var ySplit = lines.Select((l, y) => (l, y)).First(l => l.l == "").y;

            var rules = new Dictionary<string, Rules>(lines[..ySplit].Select(ParseRule));
            Part[] parts = [..lines[(ySplit+1)..].Select(ParsePart)];

            return (rules, [..parts]);

            static KeyValuePair<string, Rules> ParseRule(string line)
            {
                var nameSplit = line.Split("{");
                var rulesParts = nameSplit[1].Split(",");
                Rule[] rules = [..rulesParts[..^1].Select(ParseRulePart)];
                return new (nameSplit[0], new Rules(rules, rulesParts[^1][..^1]));
            }

            static Rule ParseRulePart(string line)
            {
                var nextRuleSplit = line.Split(":");
                var part = nextRuleSplit[0][0];
                var @operator = nextRuleSplit[0][1];
                var value = nextRuleSplit[0][2..].ToInt();
                return new Rule(part, @operator, value, nextRuleSplit[^1]);
            }

            static Part ParsePart(string line)
            {
                var split = line[1..^1].Split(",");
                var x = split[0][2..].ToInt();
                var m = split[1][2..].ToInt();
                var a = split[2][2..].ToInt();
                var s = split[3][2..].ToInt();
                return new Part(x, m, a, s);
            }
        }
    }

    record Rules(Rule[] rules, string lastRule)
    {
        public string NextRule(Part part) => rules.FirstOrDefault(r => r.Applies(part))?.nextRule ?? lastRule;
    }
    record Rule(char part, char @operator, int value, string nextRule)
    {
        public bool Applies(Part p) => (part, @operator) switch
        {
            ('x', '<') when p.x < value => true,
            ('x', '>') when p.x > value => true,
            ('m', '<') when p.m < value => true,
            ('m', '>') when p.m > value => true,
            ('a', '<') when p.a < value => true,
            ('a', '>') when p.a > value => true,
            ('s', '<') when p.s < value => true,
            ('s', '>') when p.s > value => true,
            _ => false
        };
    }
    record Part(int x, int m, int a, int s)
    {
        public int TotalValue => x + m + a + s;
    }
}