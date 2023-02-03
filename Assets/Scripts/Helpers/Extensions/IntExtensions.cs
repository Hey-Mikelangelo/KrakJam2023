public static class IntExtensions
{
    public static int CyclicPlusOne(this int val, int count)
    {
        var plusOne = val + 1;
        val = plusOne < count ? plusOne : 0;
        return val;
    }

    public static int CyclicMinusOne(this int val, int count)
    {
        var minusOne = val - 1;
        val = minusOne < 0 ? count - 1 : minusOne;
        return val;
    }
    public static void WithCyclicPlusOne(this ref int val, int count)
    {
        var plusOne = val + 1;
        val = plusOne < count ? plusOne : 0;
    }

    public static void WithCyclicMinusOne(this ref int val, int count)
    {
        var minusOne = val - 1;
        val = minusOne < 0 ? count - 1 : minusOne;
    }

}
