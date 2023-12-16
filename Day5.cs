record struct Range(long Start, long Length)
{
    public readonly long End => Start + Length;
}

record struct Ranges(long Destination, Range Range)
{
    public readonly long Map(long value) =>
        value >= Range.Start && value < Range.End
        ? Destination + (value - Range.Start)
        : value;

    public readonly Range MapRange(Range range) => new(Destination + (range.Start - Range.Start), range.Length);
}

record struct MapRanges(string To, Ranges[] Ranges)
{
    public readonly long Map(long value)
    {
        var pos = Array.BinarySearch(Ranges, new Ranges(0, new Range(value, 0)), RangesComparer.Instance);

        if (pos < 0)
        {
            pos = ~pos;
            pos -= 1;
        }

        return pos < 0 || pos >= Ranges.Length
            ? value
            : Ranges[pos].Map(value);
    }

    public readonly IEnumerable<Range> MapRange(Range range)
    {
        var pos = Array.BinarySearch(Ranges, new Ranges(0, range), RangesComparer.Instance);

        if (pos < 0)
        {
            pos = ~pos;
            pos -= 1;
        }

        if (pos < 0)
        {
            var splitRange = Ranges[0].Range;

            if (range.End < splitRange.Start)
            {
                // does not fit, return as is
                yield return range;
            }
            else
            {
                // does not fit at beginning
                yield return new Range(range.Start, splitRange.Start - range.Start);
                // map the rest recursively
                foreach (var m in MapRange(new Range(splitRange.Start, range.End - splitRange.Start)))
                {
                    yield return m;
                }
            }
        }
        else if (pos >= Ranges.Length)
        {
            var splitRange = Ranges[^1].Range;
            if (range.Start > splitRange.End)
            {
                // does not fit, return as is
                yield return range;
            }
            else
            {
                // does not fit at end
                yield return new Range(splitRange.End, range.End - splitRange.End);
                // map the start recursively
                foreach (var m in MapRange(new Range(range.Start, splitRange.End - range.Start)))
                {
                    yield return m;
                }
            }
        }
        else
        {
            var splitRange = Ranges[pos].Range;
            if (range.End <= splitRange.End)
            {
                // fits in one range
                yield return Ranges[pos].MapRange(range);
            }
            else if (range.Start >= splitRange.End)
            {
                if (pos + 1 < Ranges.Length && range.End > Ranges[pos + 1].Range.Start)
                {
                    var next = Ranges[pos + 1].Range;
                    // does not fit at start
                    yield return new Range(range.Start, next.Start - range.Start);
                    // map the rest recursively
                    foreach (var m in MapRange(new Range(next.Start, range.End - next.Start)))
                    {
                        yield return m;
                    }
                }
                else
                {
                    // does not fit, return as is
                    yield return range;
                }
            }
            else
            {
                var sr = new Range(range.Start, splitRange.End - range.Start);
                // begin fits in one range
                yield return Ranges[pos].MapRange(sr);
                // map the rest recursively
                foreach (var m in MapRange(new Range(splitRange.End, range.End - splitRange.End)))
                {
                    yield return m;
                }
            }
        }
    }
}

class RangesComparer : Comparer<Ranges>
{
    public override int Compare(Ranges x, Ranges y) => Comparer<long>.Default.Compare(x.Range.Start, y.Range.Start);
    public static RangesComparer Instance { get; } = new RangesComparer();
}

static partial class Aoc2023
{
    public static void Day5()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                seeds: 79 14 55 13

                seed-to-soil map:
                50 98 2
                52 50 48

                soil-to-fertilizer map:
                0 15 37
                37 52 2
                39 0 15

                fertilizer-to-water map:
                49 53 8
                0 11 42
                42 0 7
                57 7 4

                water-to-light map:
                88 18 7
                18 25 70

                light-to-temperature map:
                45 77 23
                81 45 19
                68 64 13

                temperature-to-humidity map:
                0 69 1
                1 0 69

                humidity-to-location map:
                60 56 37
                56 93 4
                """.ToLines();
            Part1(input).Should().Be(35);
            Part2(input).Should().Be(46);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(157211394L);
            Part2(input).Should().Be(50855035L);
        }

        long Part1(string[] lines)
        {
            var seeds = ParseSeeds();

            TraverseMaps(lines, MoveToNextCategory);

            return seeds.Min();

            void MoveToNextCategory(MapRanges map)
            {
                for (int i = 0; i < seeds.Length; i++)
                {
                    seeds[i] = map.Map(seeds[i]);
                }
            }

            long[] ParseSeeds() => lines[0].Split(": ")[1].ToLongs(" ");
        }

        long Part2(string[] lines)
        {
            var seedRanges = ParseSeedRanges();
            var maps = ParseMaps(lines);

            TraverseMaps(lines, MoveToNextCategory);

            return seedRanges.Min(x => x.Start);

            void MoveToNextCategory(MapRanges map) =>
                seedRanges = seedRanges.SelectMany(x => map.MapRange(x)).ToList();

            List<Range> ParseSeedRanges()
            {
                var n = lines[0].Split(": ")[1].ToLongs(" ");
                List<Range> ranges = [];
                for (int i = 0; i < n.Length; i += 2)
                {
                    ranges.Add(new Range(n[i], n[i+1]));
                }
                return ranges;
            }
        }

        static void TraverseMaps(string[] lines, Action<MapRanges> action)
        {
            var maps = ParseMaps(lines);
            var mapping = "seed";
            while (mapping != "location")
            {
                var map = maps[mapping];
                action(map);
                mapping = map.To;
            }
        }

        static Dictionary<string, MapRanges> ParseMaps(string[] lines)
        {
            Dictionary<string, MapRanges> maps = [];
            for (var i = 2; i < lines.Length; i++)
            {
                var (id, ranges) = ParseMap(ref i);
                maps[id] = ranges;
            }
            return maps;

            (string id, MapRanges ranges) ParseMap(ref int i)
            {
                var mapping = lines[i].Split(" ")[0].Split("-to-");
                i++;
                var ranges = new List<Ranges>();
                while (i < lines.Length && lines[i] != "")
                {
                    var n = lines[i].ToLongs(" ");
                    ranges.Add(new Ranges(n[0], new Range(n[1], n[2])));
                    i++;
                }

                return (mapping[0], new MapRanges(mapping[1], [.. ranges.OrderBy(x => x.Range.Start)]));
            }
        }
    }
}