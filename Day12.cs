static partial class Aoc2023
{
    public static void Day12()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;
        Console.WriteLine(day);

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                ???.### 1,1,3
                .??..??...?##. 1,1,3
                ?#?#?#?#?#?#?#? 1,3,1,6
                ????.#...#... 4,1,1
                ????.######..#####. 1,6,5
                ?###???????? 3,2,1
                """.ToLines();
            Part1(input).Should().Be(21);
            Part2(input).Should().Be(525152);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(6935);
            Part2(input).Should().Be(3920437278260);
        }

        long Part1(string[] lines) => lines.Select(ParseSpring).Select(CountArrangements).Sum();

        long Part2(string[] lines) => lines.Select(ParseSpring).Select(Unfold).Select(CountArrangements).Sum();

        static (char[], int[]) ParseSpring(string line)
        {
            var parts = line.Split(" ");
            var conditions = parts[0].ToCharArray();
            var broken = parts[1].ToInts(",");
            return (conditions, broken);
        }

        static (char[], int[]) Unfold((char[], int[]) spring)
        {
            var (conditions, broken) = spring;
            char[] unfoldedConditions = [..conditions, '?', ..conditions, '?', ..conditions, '?', ..conditions, '?', ..conditions];
            int[] unfoldedBroken = [..broken, ..broken, ..broken, ..broken, ..broken];
            return (unfoldedConditions, unfoldedBroken);
        }

        static long CountArrangements((char[], int[]) spring)
        {
            var countArrangements = 0L;
            var (conditions, broken) = spring;
            
            var possibleOffsets = conditions.Select((c, i) => (c, i)).Where(x => x.c != '.').Select(x => x.i).ToList();

            var work = new List<(int brokenIndex, int offset, long count)>();
            
            // enqueue all possible offsets for first broken
            EnqueueWork(0, 0, 0, 1);

            while (work.Count > 0)
            {
                var (brokenIndex, offset, count) = work[0]; work.RemoveAt(0);

                var brokenLength = broken[brokenIndex];
                var offsetEnd = offset + brokenLength;

                // check if we can place the broken here
                if (!((offset == 0 || conditions[offset - 1] != '#')
                    && (offsetEnd == conditions.Length || offsetEnd < conditions.Length && conditions[offsetEnd] != '#')
                    && !conditions.AsSpan(offset, brokenLength).Contains('.')))
                {
                    continue;
                }

                // did we reach the end?
                if (brokenIndex == broken.Length - 1)
                {
                    // if there are # left, we did not find a solution
                    if (!conditions.AsSpan(offsetEnd).Contains('#'))
                    {
                        countArrangements += count;
                    }
                }
                else
                {
                    // find next possible that should fit
                    var nextPossibleIndex = possibleOffsets.BinarySearch(offsetEnd+1);
                    if (nextPossibleIndex < 0) nextPossibleIndex = ~nextPossibleIndex;
                    
                    // enqueue next possible offset for next brokens
                    EnqueueWork(brokenIndex + 1, offsetEnd, nextPossibleIndex, count);
                }
            }

            return countArrangements;

            void EnqueueWork(int bi, int offsetEnd, int nextPossibleIndex, long count)
            {
                for (var i = nextPossibleIndex; i < possibleOffsets.Count; i++)
                {
                    var nextOffset = possibleOffsets[i];

                    // if we find # in between, we must stop
                    if (conditions.AsSpan(offsetEnd, nextOffset - offsetEnd).Contains('#')) 
                    {
                        break;
                    }

                    // find duplicates
                    var wi = work.FindIndex(w => w.brokenIndex == bi && w.offset == nextOffset);
                    if (wi >= 0)
                    {
                        work[wi] = work[wi] with { count = work[wi].count + count };
                    }
                    else
                    {
                        work.Add((bi, nextOffset, count));
                    }
                }
            }
        }
    }
}