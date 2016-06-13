using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorySplitter
{
    /// <summary>
    /// Represents container of helper methods for directory operations.
    /// </summary>
    public interface IDirectoryHelper
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
        /// A collection of the full names (including relative paths) of the subdirectories that match
        /// the search pattern. 
        /// </returns>
        IEnumerable<string> GetDirectories(string path, string searchPattern, SearchOption searchOption);

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        string GetRelativePath(string fromPath, string toPath);

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        string AppendDirectorySeparatorChar(string path);

        /// <summary>
        /// Appends the directory separator character to the specified path if it does not already trail with it.
        /// </summary>
        /// <param name="path">The path to append.</param>
        /// <param name="force">Whether to append directory separator to a path with an extension.</param>
        /// <returns>The specified path appended with the directory separator character if it does not already trail with it.</returns>
        string AppendDirectorySeparatorChar(string path, bool force);
    }
}
