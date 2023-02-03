namespace Utilities
{
    public static class StringExtensions
    {
        public static string TrimStart(this string value, char c, int maxCount)
        {
            int removeCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char letter = value[i];
                if (letter != c)
                {
                    break;
                }
                removeCount++;
            }
            return value.Remove(0, removeCount);
        }
    }
}