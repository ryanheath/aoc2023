static class MathExtensions
{
    public static T Gcd<T>(T a, T b) where T : INumber<T>
    {
        while (b != T.Zero) (b, a) = (a % b, b);

        return a;
    }

    public static T Lcm<T>(T a, T b) where T : INumber<T> => b / Gcd(a, b) * a;

    public static T Gcd<T>(this IEnumerable<T> numbers) where T : INumber<T> => numbers.Aggregate(Gcd);

    public static T Lcm<T>(this IEnumerable<T> numbers) where T : INumber<T>  => numbers.Aggregate(Lcm);

    public static int PositiveMod(int a, int b) => (a % b + b) % b;
}
