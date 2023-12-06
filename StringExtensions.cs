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
                group = new List<string>();
            }
            else
            {
                group.Add(line);
            }
        }

        yield return group;
    }

    static public string[] ToLines(this string input)
    {
        return input.Split("\r\n");
    }

    static public int[] ToInts(this string input)
    {
        return input.ToInts("\r\n");
    }

    static public int[] ToInts(this string input, string splitter)
    {
        return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
    }

    static public long[] ToLongs(this string input, string splitter)
    {
        return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
    }
    static public ulong[] ToULongs(this string input, string splitter)
    {
        return input.Split(splitter, StringSplitOptions.RemoveEmptyEntries).Select(ulong.Parse).ToArray();
    }

    static public int[] ToInts(this string[] input)
    {
        return input.Select(int.Parse).ToArray();
    }

    static public string Sort(this string input)
    {
        return string.Concat(input.OrderBy(c => c));
    }

    static public int ToInt(this string input)
    {
        return int.Parse(input);
    }

    static public long ToLong(this string input)
    {
        return long.Parse(input);
    }

    static public string ToBits(this int input)
    {
        return Convert.ToString(input, 2);
    }

    static public string ToBits(this byte input)
    {
        return Convert.ToString(input, 2);
    }

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

    public static string ReverseString(this string s) => new(s.Reverse().ToArray());
}
