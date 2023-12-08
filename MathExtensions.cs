static class MathExtensions
{
    public static long Lcm(long a, long b)
    {
        return b / Gcd(a, b) * a;
    }

    public static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            var t = b;
            b = a % b;
            a = t;
        }

        return a;
    }

    public static long Gcd(this IEnumerable<long> numbers)
    {
        return numbers.Aggregate(Gcd);
    }

    public static long Lcm(this IEnumerable<long> numbers)
    {
        return numbers.Aggregate(Lcm);
    }
}
