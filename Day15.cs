static partial class Aoc2023
{
    public static void Day15
    ()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7
                """.ToLines();
            Part1(input).Should().Be(1320);
            Part2(input).Should().Be(145);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(510388);
            Part2(input).Should().Be(291774);
        }

        int Part1(string[] lines) => lines.Select(l => l.Split(',')).SelectMany(l => l).Select(HASH).Sum();
        int Part2(string[] lines) => lines.Select(l => l.Split(',').Select(ParseStep)).Select(HASHMAP).Sum();

        static int HASH(string s) => s.Aggregate(0, (h, c) => (h + c) * 17 % 256);

        static int HASHMAP(IEnumerable<(string label, bool add, int? focalLength)> steps)
        {
            var boxes = new List<(string label, int focalLength)>[256];
            for (var i = 0; i < 256; i++) boxes[i] = [];

            foreach (var (label, add, focalLength) in steps)
            {
                var hash = HASH(label);
                var box = boxes[hash];
                var pos = box.FindIndex(b => b.label == label);

                if (add)
                {
                    var newStep = (label, focalLength ?? 0);
                    if (pos == -1) box.Add(newStep);
                    else box[pos] = newStep;
                }
                else if (pos != -1) box.RemoveAt(pos);
            }

            return boxes.SelectMany((box, i) => box.Select((step, j) => (i + 1) * (j + 1) * step.focalLength)).Sum();
        }

        static (string label, bool add, int? focalLength) ParseStep(string step)
        {
            var groups = StepRegex().Match(step).Groups;
            return (groups["label"].Value, groups["add"].Value == "=", groups["focalLength"].Value.ToNullableInt());
        }
    }

    [GeneratedRegex(@"(?<label>.+)(?<add>[=-])(?<focalLength>\d+)?")]
    private static partial Regex StepRegex();
}