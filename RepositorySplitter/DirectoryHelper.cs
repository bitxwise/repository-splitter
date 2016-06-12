using System;
using System.IO;

namespace RepositorySplitter
{
    /// <summary>
    /// Contains helper methods for directory operations.
    /// </summary>
    public class DirectoryHelper
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="fromPath"/> or <paramref name="toPath"/> is <c>null</c>.</exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Credit: http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/32113484#32113484
        /// </remarks>
        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath))
                throw new ArgumentNullException("fromPath");

            if (string.IsNullOrEmpty(toPath))
                throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme)
                return toPath;

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            
            return relativePath;
        }

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        public static string AppendDirectorySeparatorChar(string path)
        {
            return AppendDirectorySeparatorChar(path, false);
        }

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <param name="force">Whether to append directory separator to a path with an extension.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        public static string AppendDirectorySeparatorChar(string path, bool force)
        {
            // Append a slash only if the path is a directory and does not have a slash
            if ((force || !Path.HasExtension(path)) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;

            return path;
        }
    }
}
