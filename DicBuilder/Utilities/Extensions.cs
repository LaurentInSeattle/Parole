namespace Lyt.Tools
{
    using System.Collections.Generic;

    public static class Extensions
    {
        public static bool IsEmpty<T>(this ICollection<T> collection) 
            => ((collection == null) || (collection.Count == 0));
    }
}
