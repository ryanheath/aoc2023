static class StringExtensions
{
    static public IEnumerable<List<string>> GroupLines(this string [] lines)
    {
        var group = new List<string>();

        foreach(var line in lines)
        {
            if (line == "")
            {
                yield return group;
                group = [];
            }
            else
            {
                group.Add(line);
            }
        }

        yield return group;
    }

    static public string[] ToLines(this string input) => input.Split("\r\n");

    static public int[] ToInts(this string input) => input.ToInts("\r\n");

    static public int[] ToInts(this string input, string splitter) => [.. input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)];

    static public IEnumerable<int[]> ToInts(this string[] lines, string splitter) => lines.Select(line => line.ToInts(splitter));

    static public long[] ToLongs(this string input, string splitter) 
        => [.. input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse)];
    static public ulong[] ToULongs(this string input, string splitter) 
        => [.. input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse)];

    static public int[] ToInts(this string[] input) => [.. input.Select(int.Parse)];

    static public string Sort(this string input) => string.Concat(input.OrderBy(c => c));

    static public int ToInt(this string input) => int.Parse(input);
    static public int ToIntFromHex(this string input) => int.Parse(input, System.Globalization.NumberStyles.HexNumber);
    static public int? ToNullableInt(this string input) => string.IsNullOrEmpty(input) ? null : int.Parse(input);

    static public long ToLong(this string input) => long.Parse(input);

    static public string ToBits(this int input) => Convert.ToString(input, 2);

    static public string ToBits(this byte input) => Convert.ToString(input, 2);

    static public int IntFromBits(this string input) => input.AsSpan().IntFromBits();

    static public int IntFromBits(this ReadOnlySpan<char> input)
    {
        var v = 0;
        foreach (var c in input)
        {
            v <<= 1;
            if (c == '1')
            {
                v |= 1;
            }
        }

        return v;
    }

    public static string ReverseString(this string s) => new([..s.Reverse()]);
}
