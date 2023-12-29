

static partial class Aoc2023
{
    public static void Day25()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input =
                """
                jqt: rhn xhk nvd
                rsh: frs pzl lsr
                xhk: hfx
                cmg: qnr nvd lhk bvb
                rhn: xhk bvb hfx
                bvb: xhk hfx
                pzl: lsr hfx nvd
                qnr: nvd
                ntq: jqt hfx bvb xhk
                nvd: lhk
                lsr: lhk
                rzs: qnr cmg lsr rsh
                frs: qnr lhk lsr
                """.ToLines();
            Part1(input).Should().Be(54);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(552695);
        }

        int Part1(string[] lines) => CountGroups(ParseComponents(lines));

        static int CountGroups(Dictionary<string, Component> dict)
        {
            var group1 = new List<Component>();
            var group2 = new List<Component>();
            var pairs = FindThreePairs(dict);

            foreach(var (c1, c2) in pairs)
            {
                if (!group1.Any())
                {
                    group1.Add(c1); group2.Add(c2);
                }
                c1.Disconnect(c2);
            }

            var components = dict.Values.Except([..group1, ..group2]).ToHashSet();

        restart:
            foreach(var component in components)
            {
                if (component.Connects.Any(c => group1.Contains(c)))
                {
                    group1.Add(component);
                    components.Remove(component);
                    goto restart;
                }
                else if (component.Connects.Any(c => group2.Contains(c)))
                {
                    group2.Add(component);
                    components.Remove(component);
                    goto restart;
                }
            }

            return group1.Count * group2.Count;

            static IEnumerable<(Component, Component)> FindThreePairs(Dictionary<string, Component> dict)
            {
                var pairs = new Dictionary<(Component, Component), int>();

                foreach (var component in dict.Values.Take(100)) // take just enough to find 3 pairs
                foreach (var paths in FindFurtests(component))
                foreach (var path in paths)
                for (var i = 0; i < path.Count - 1; i++)
                for (var j = i + 1; j < path.Count; j++)
                {
                    var pair = string.CompareOrdinal(path[i].Name, path[j].Name) > 0 ? (path[i], path[j]) : (path[j], path[i]);
                    if (pairs.TryGetValue(pair, out var count))
                        pairs[pair] = count + 1;
                    else
                        pairs[pair] = 1;
                }

                return pairs.OrderByDescending(p => p.Value).Select(x => x.Key).Take(3);

                static IEnumerable<IGrouping<int, List<Component>>> FindFurtests(Component value)
                {
                    var paths = new List<List<Component>>();
                    var seenPaths = new HashSet<string>();

                    var visited = new HashSet<Component>();
                    var queue = new Queue<(Component component, List<Component>)>();
                    queue.Enqueue((value, [value]));

                    while (queue.Count > 0)
                    {
                        var (component, path) = queue.Dequeue();
                        visited.Add(component);
                        foreach (var connect in component.Connects)
                            if (visited.Add(connect))
                                queue.Enqueue((connect, [.. path, connect]));
                            else if (seenPaths.Add(path.Select(p => p.Name).Aggregate((a, b) => $"{b}-{a}")))
                                paths.Add([.. path]);
                    }

                    return paths
                        .GroupBy(p => p.Count)
                        .OrderByDescending(g => g.Key)
                        .Take(1);
                }
            }
        }

        static Dictionary<string, Component> ParseComponents(string[] lines)
        {
            var components = new Dictionary<string, Component>();
            foreach (var line in lines)
                ParseComponent(line);
            return components;

            void ParseComponent(string line)
            {
                var parts = line.Split(": ");
                var component = GetOrCreateComponent(parts[0]);
                foreach(var connect in parts[1].Split(" "))
                    component.ConnectsTo(GetOrCreateComponent(connect));
            }
            Component GetOrCreateComponent(string name)
            {
                if (!components.TryGetValue(name, out var component))
                    components[name] = component = new Component(name);
                return component;
            }
        }
    }

    record Component(string Name)
    {
        public HashSet<Component> Connects { get; } = [];

        public void ConnectsTo(Component other)
        {
            Connects.Add(other);
            other.Connects.Add(this);
        }

        public void Disconnect(Component other)
        {
            other.Connects.Remove(this);
            Connects.Remove(other);
        }
    }
}