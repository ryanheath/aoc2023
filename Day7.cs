static partial class Aoc2023
{
    public static void Day7()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                32T3K 765
                T55J5 684
                KK677 28
                KTJJT 220
                QQQJA 483
                """.ToLines();
            Part1(input).Should().Be(6440);
            Part2(input).Should().Be(5905);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(247815719);
            Part2(input).Should().Be(248747492);
        }

        int Part1(string[] lines) => PlayCards(lines, withJoker: false);
        int Part2(string[] lines) => PlayCards(lines, withJoker: true);

        int PlayCards(string[] lines, bool withJoker) 
            => ParseHands(lines, withJoker)
                .OrderBy(x => (x.hand, x.type), TypesComparer.Instance(withJoker))
                .Select((x, i) => x.bid * (i + 1))
                .Sum();

        static (string hand, int bid, (char card, int count)[] type)[] ParseHands(string[] lines, bool withJoker) => [..lines.Select(x => ParseHand(x, withJoker))];
        static (string hand, int bid, (char card, int count)[] type) ParseHand(string line, bool withJoker)
        {
            var split = line.Split(" ");
            return (split[0], split[1].ToInt(), withJoker ? HandTypeWithJoker(split[0]) : HandType(split[0]));
        }
        static (char card, int count)[] HandType(string hand)
            => [.. hand
                .GroupBy(x => x, (k, c) => (card: k, count: c.Count()))
                .OrderByDescending(x => x.count)];
        static (char card, int count)[] HandTypeWithJoker(string hand)
        {
            var jokers = hand.Count(x => x == 'J');
            if (jokers == 0) return HandType(hand);

            if (jokers == 5)
            {
                return [('J', 5)];
            }

            var groups = HandType(hand.Replace("J", ""));
            groups[0] = (groups[0].card, groups[0].count + hand.Count(x => x == 'J'));

            return groups;
        }
    }

    class TypesComparer(bool withJoker) : Comparer<(string hand, (char card, int count)[] type)>
    {
        public override int Compare((string hand, (char card, int count)[] type) x, (string hand, (char card, int count)[] type) y)
        {
            var countDifference = Comparer<int>.Default.Compare(x!.type[0].count, y!.type[0].count);
            return countDifference != 0
                ? countDifference
                : x.type.Length == y.type.Length
                    ? HandComparer.Instance(withJoker).Compare(x.hand, y.hand)
                    : x.type.Length < y.type.Length ? 1 : -1;
        }

        static public TypesComparer Instance(bool withJoker) => withJoker ? InstanceWithJoker : InstanceWithoutJoker;

        static private TypesComparer InstanceWithoutJoker { get; } = new(false);
        static private TypesComparer InstanceWithJoker { get; } = new(true);
    }

    class HandComparer(bool withJoker) : Comparer<string>
    {
        public override int Compare(string? x, string? y)
        {
            var difference = 0;

            for (var i = 0; i < 5; i++)
            {
                difference = CardComparer.Instance(withJoker).Compare(x![i], y![i]);
                if (difference != 0) return difference;
            }

            return difference;
        }

        static public HandComparer Instance(bool withJoker) => withJoker ? InstanceWithJoker : InstanceWithoutJoker;

        static private HandComparer InstanceWithoutJoker { get; } = new(false);
        static private HandComparer InstanceWithJoker { get; } = new(true);
    }

    class CardComparer(bool withJoker) : Comparer<char>
    {
        public override int Compare(char x, char y) => Comparer<int>.Default.Compare(CardValue(x), CardValue(y));

        static public CardComparer Instance(bool withJoker) => withJoker ? InstanceWithJoker : InstanceWithoutJoker;

        static private CardComparer InstanceWithoutJoker { get; } = new(false);
        static private CardComparer InstanceWithJoker { get; } = new(true);
     
        int CardValue(char c) 
                => c switch
                {
                    'A' => 14,
                    'K' => 13,
                    'Q' => 12,
                    'J' => withJoker ? 1 : 11,
                    'T' => 10,
                    _ => c - '0'
                };
    }
}