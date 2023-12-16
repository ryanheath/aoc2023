static partial class Aoc2023
{
    public static void Day4()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
                Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
                Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
                Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
                Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
                Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
                """.ToLines();
            Part1(input).Should().Be(13);
            Part2(input).Should().Be(30);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(27845);
            Part2(input).Should().Be(9496801);
        }

        int Part1(string[] lines) => CalculateScore(lines).Sum();
        int Part2(string[] lines) => CountScratchCards(lines);

        static IEnumerable<int> CalculateScore(string[] lines) =>
            ParseScratchCards(lines).Select(x => (int)Math.Pow(2, x.Value - 1));

        static int CountScratchCards(string[] lines)
        {
            var scratchCards = ParseScratchCards(lines);
            var queue = new Queue<int>(scratchCards.Keys);
            var count = 0;
            while (queue.Count > 0)
            {
                var cardId = queue.Dequeue();
                count++;
                var copies = scratchCards[cardId];
                for (var i = 1; i <= copies; i++)
                {
                    queue.Enqueue(cardId + i);
                }
            }
            return count;
        }

        static IDictionary<int, int> ParseScratchCards(string[] lines)
        {
            var scratchCards = new Dictionary<int, int>();
            foreach (var line in lines)
            {
                var splitCard = line.Split(": ");

                var cardId = splitCard[0][5..].ToInt();

                var splitNumbers = splitCard[1].Split(" | ");
                var winningNumbers = splitNumbers[0].ToInts(" ");
                var gameNumbers = splitNumbers[1].ToInts(" ");
                var matchedNumbers = winningNumbers.Intersect(gameNumbers).Count();

                scratchCards[cardId] = matchedNumbers;
            }
            return scratchCards;
        }
    }
}