namespace CrossingFingers_Wave.Extensions
{
    public static class FloatExtensions
    {
        public static bool Between(this float num, float lower, float upper)
        {
            return num.Between(lower, upper, true);
        }

        public static bool Between(this float num, float lower, float upper, bool inclusive)
        {
            bool isBetween;

            if (inclusive)
            {
                isBetween = lower <= num && num <= upper;
            }
            else
            {
                isBetween = lower < num && num < upper;
            }

            return isBetween;
        }
    }
}
