static partial class Aoc2023
{
    public static void Day1()
    {
        ComputeExample(); Compute();

        void ComputeExample()
        {
            var input = 
                """
                1abc2
                pqr3stu8vwx
                a1b2c3d4e5f
                treb7uchet
                """.ToLines();
            Part1(input).Should().Be(142);

            input = 
                """
                two1nine
                eightwothree
                abcone2threexyz
                xtwone3four
                4nineeightseven2
                zoneight234
                7pqrstsixteen
                """.ToLines();
            Part2(input).Should().Be(281);
        }

        void Compute()
        {
            var input = File.ReadAllLines("day1.txt");
            Part1(input).Should().Be(55208);
            Part2(input).Should().Be(54578);
        }

        int Part1(string[] lines) => lines.Select(Calibration).Sum();

        int Part2(string[] lines) => lines.Select(CalibrationRegex).Sum();

        static int Calibration(string line)
        {
            var sv = SearchValues.Create("123456789");
            var span = line.AsSpan();
            var fi = span.IndexOfAny(sv);
            var li = span.LastIndexOfAny(sv);
            var d = ToDigit(line[fi]) * 10 + ToDigit(line[li]);
            return d;

            static int ToDigit(char c) => c - '0';
        }

        static int CalibrationRegex(string line)
        {
            var regexpr = "[1-9]|one|two|three|four|five|six|seven|eight|nine";
            var fsv = new Regex(regexpr);
            var lsv = new Regex(regexpr, RegexOptions.RightToLeft);
            return 
                ToDigit(fsv.Match(line).Value) * 10 + 
                ToDigit(lsv.Match(line).Value);

            static int ToDigit(string value) => value switch
            {
                "one" => 1,
                "two" => 2,
                "three" => 3,
                "four" => 4,
                "five" => 5,
                "six" => 6,
                "seven" => 7,
                "eight" => 8,
                "nine" => 9,
                _ => value[0] - '0'
            };
        }
    }
}