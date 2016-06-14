using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepositorySplitter
{
    /// <summary>
    /// Contains helper methods for directory operations.
    /// </summary>
    public class WindowsDirectoryHelper : IDirectoryHelper
    {
        /// <summary>
        /// Gets the names of the subdirectories (including their paths) that match the
        /// specified search pattern in the current directory, and optionally searches
        /// subdirectories.
        /// </summary>
        /// <param name="path">The path to search.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. The parameter
        /// cannot end in two periods ("..") or contain two periods ("..") followed by
        /// System.IO.Path.DirectorySeparatorChar or System.IO.Path.AltDirectorySeparatorChar,
        /// nor can it contain any of the characters in System.IO.Path.InvalidPathChars.
        /// </param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation
        /// should include all subdirectories or only the current directory.
        /// </param>
        /// <returns>
        /// A collection of the names (including relative paths) of the subdirectories that match
        /// the search pattern, without a directory separator at the end of each name.
        /// </returns>
        public IEnumerable<string> GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            // get relative paths for each directory
            var directories = Directory.GetDirectories(path, searchPattern, searchOption);
            for (int i = 0; i < directories.Length; i++)
                directories[i] = GetRelativePath(path, directories[i]);

            return directories;
        }

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
        /// Credit: Adjusted from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path/32113484#32113484
        /// </remarks>
        public string GetRelativePath(string fromPath, string toPath)
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
            
            return relativePath.TrimEnd(Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        public string AppendDirectorySeparatorChar(string path)
        {
            return AppendDirectorySeparatorChar(path, false);
        }

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <param name="force">Whether to append directory separator to a path with an extension.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        public string AppendDirectorySeparatorChar(string path, bool force)
        {
            // Append a slash only if the path is a directory and does not have a slash
            if ((force || !Path.HasExtension(path)) && !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                return path + Path.AltDirectorySeparatorChar;

            return path;
        }
    }
}
