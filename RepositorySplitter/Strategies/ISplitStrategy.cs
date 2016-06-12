using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorySplitter
{
    /// <summary>
    /// Represents a strategy for splitting a repository.
    /// </summary>
    public interface ISplitStrategy
    {
        /// <summary>
        /// Splits the specified repository.
        /// </summary>
        /// <param name="repository">Directory path for the repository to split.</param>
        void Split(string repository);
    }
}
