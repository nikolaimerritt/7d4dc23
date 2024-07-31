namespace PirateConquest.Utils
{
    public static class ListExtensions
    {
        private static readonly Random Random = new();

        public static List<T> ShuffleInPlace<T>(this List<T> list)
        {
            int count = list.Count;
            while (count > 1)
            {
                count--;
                int k = Random.Next(count + 1);
                (list[count], list[k]) = (list[k], list[count]);
            }
            return list;
        }
    }
}
