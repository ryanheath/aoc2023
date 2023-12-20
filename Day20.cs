static partial class Aoc2023
{
    public static void Day20()
    {
        var day = MethodBase.GetCurrentMethod()!.Name;

        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                broadcaster -> a, b, c
                %a -> b
                %b -> c
                %c -> inv
                &inv -> a
                """.ToLines();
            Part1(input).Should().Be(32000000);
            input = 
                """
                broadcaster -> a
                %a -> inv, con
                &inv -> b
                %b -> con
                &con -> output
                """.ToLines();
            Part1(input).Should().Be(11687500);
        }

        void Compute()
        {
            var input = File.ReadAllLines($"{day.ToLowerInvariant()}.txt");
            Part1(input).Should().Be(812609846);
            Part2(input).Should().Be(245114020323037);
        }

        int Part1(string[] lines) => PushButtonTimes(1000, ParseModules(lines));
        long Part2(string[] lines) => DebugPushButton("rx", ParseModules(lines));

        static long DebugPushButton(string trackOutput, Dictionary<string, ModuleBase> modules)
        {
            // skip first parent modules
            var inputsForTrackOutput1 = modules.Values.Where(x => x.Outputs.Contains(trackOutput));
            // use second parent modules
            ModuleBase[] inputsForTrackOutput2 = [..modules.Values.Where(x => x.Outputs.Any(y => inputsForTrackOutput1.Any(z => z.Name == y)))];

            var counts = new Dictionary<string, long>();

            for (var times = 1; ; times++)
            {
                PushButton(modules);

                foreach (var conjunction in inputsForTrackOutput2.OfType<ConjunctionModule>())
                    if (conjunction.PulseHighCount == 1 && !counts.ContainsKey(conjunction.Name))
                        counts[conjunction.Name] = times;

                if (counts.Count == inputsForTrackOutput2.Length)
                    break;
            }

            return counts.Values.Lcm();
        }

        static int PushButtonTimes(int times, Dictionary<string, ModuleBase> modules)
        {
            for (var i = 0; i < times; i++)
                PushButton(modules);

            var totalLow = modules.Values.Sum(x => x.PulseLowCount);
            var totalHigh = modules.Values.Sum(x => x.PulseHighCount);

            return totalLow * totalHigh;
        }

        static void PushButton(Dictionary<string, ModuleBase> modules)
        {
            var output = new OutputModule();
            var bus = new Queue<(string sender, string receiver, Pulse)>();
            bus.Enqueue(("button", modules["button"].Outputs[0], Pulse.Low));

            while (bus.Count > 0)
            {
                var (senderName, receiverName, pulse) = bus.Dequeue();
                var sender = modules[senderName];
                if (!modules.TryGetValue(receiverName, out var receiver))
                    receiver = output;

                var result = sender.SendPulse(receiver, pulse);

                if (result == Pulse.None) continue;

                foreach (var outputName in receiver.Outputs)
                    bus.Enqueue((receiverName, outputName, result));
            }
        }

        static Dictionary<string, ModuleBase> ParseModules(string[] lines)
        {
            var modules = new Dictionary<string, ModuleBase>(lines.Select(ParseModule));

            modules["button"] = new ButtonModule();

            // register inputs for conjunction modules
            foreach (var module in modules.Values.OfType<ConjunctionModule>())
                module.RegisterInputs([..modules.Values.Where(x => x.Outputs.Contains(module.Name)).Select(x => x.Name)]);

            return modules;

            static KeyValuePair<string, ModuleBase> ParseModule(string line)
            {
                var parts = line.Split(" -> ");
                var name = parts[0];
                var outputs = parts[1].Split(", ");
                
                if (name == "broadcaster")
                {
                    return new(name, new BroadcasterModule(outputs));
                }
                else if (name[0] == '%')
                {
                    return new(name[1..], new FlipFlopModule(name[1..], outputs));
                }
                else if (name[0] == '&')
                {
                    return new(name[1..], new ConjunctionModule(name[1..], outputs));
                }
                else
                {
                    throw new UnreachableException();
                }
            }
        }
    }

    abstract record class ModuleBase(string Name, string[] Outputs)
    {
        public int PulseHighCount { get; private set; }
        public int PulseLowCount { get; private set; }

        public Pulse SendPulse(ModuleBase receiver, Pulse pulse)
        {
            if (pulse == Pulse.High) PulseHighCount++;
            if (pulse == Pulse.Low) PulseLowCount++;
            return receiver.ReceivePulse(this, pulse);
        }

        abstract protected Pulse ReceivePulse(ModuleBase sender, Pulse pulse);
    }

    record class OutputModule() : ModuleBase("output", [])
    {
        override protected Pulse ReceivePulse(ModuleBase sender, Pulse pulse) => Pulse.None;
    }

    abstract record class PassthroughModule(string Name, string[] Outputs) : ModuleBase(Name, Outputs)
    {
        override protected Pulse ReceivePulse(ModuleBase sender, Pulse pulse) => pulse;
    }
    record class ButtonModule() : PassthroughModule("button", ["broadcaster"]);
    record class BroadcasterModule(string[] Outputs) : PassthroughModule("broadcaster", Outputs);

    record class FlipFlopModule(string Name, string[] Outputs) : ModuleBase(Name, Outputs)
    {
        private bool on = false;

        override protected Pulse ReceivePulse(ModuleBase _, Pulse pulse)
        {
            if (pulse == Pulse.High)
            {
                return Pulse.None;
            }
            
            on = !on;
            return on ? Pulse.High : Pulse.Low;
        }
    }

    record class ConjunctionModule(string Name, string[] Outputs) : ModuleBase(Name, Outputs)
    {
        private Dictionary<string, Pulse> memory = new();

        public void RegisterInputs(string[] inputs) => memory = inputs.ToDictionary(x => x, x => Pulse.Low);

        override protected Pulse ReceivePulse(ModuleBase sender, Pulse pulse)
        {
            memory[sender.Name] = pulse;
            return memory.Values.All(x => x == Pulse.High) ? Pulse.Low : Pulse.High;
        }
    }

    enum Pulse
    {
        None, 
        High,
        Low
    }
}