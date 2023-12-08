static class MathExtensions
{
    public static long Gcd(long a, long b)
    {
        while (b != 0) (b, a) = (a % b, b);

        return a;
    }

    public static long Lcm(long a, long b) => b / Gcd(a, b) * a;

    public static long Gcd(this IEnumerable<long> numbers) => numbers.Aggregate(Gcd);

    public static long Lcm(this IEnumerable<long> numbers) => numbers.Aggregate(Lcm);
}
