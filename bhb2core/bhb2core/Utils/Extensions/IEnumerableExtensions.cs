using System.Collections.Generic;
using System.Linq;

namespace bhb2core.Utils.Extensions
{
  internal static class IEnumerableExtensions
  {
    public static IEnumerable<T> Clone<T>(this IEnumerable<T> target)
    {
      return target.ToList();
    }
  }
}
