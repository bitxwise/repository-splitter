using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositorySplitter
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Generates a string concatenated with all values in specified collection delimited by the specified separator.
        /// </summary>
        /// <param name="values">Values to concatenate.</param>
        /// <param name="separator">Delimiter to separate concatenation.</param>
        /// <returns>String concatenated with all values in specified collection delimited by specified separator.</returns>
        public static string Implode(this IEnumerable<string> values, string separator)
        {
            return values.Implode(separator, o => o);
        }

        /// <summary>
        /// Generates a string concatenated with all values in specified collection delimited by the specified separator.
        /// </summary>
        /// <param name="values">Values to concatenate.</param>
        /// <param name="separator">Delimiter to separate concatenation.</param>
        /// <param name="value">Function that provides a System.String representation for each value in the specified collection.</param>
        /// <returns>String concatenated with all values in specified collection delimited by specified separator.</returns>
        public static string Implode<T>(this IEnumerable<T> values, string separator, Func<T, string> value)
        {
            var sb = new StringBuilder();

            using (var iter = values.Where(v => v != null).GetEnumerator())
            {
                if (iter.MoveNext())
                    sb.Append(value(iter.Current));

                while (iter.MoveNext())
                {
                    sb.Append(separator);
                    sb.Append(value(iter.Current));
                }
            }

            return sb.ToString();
        }
    }
}