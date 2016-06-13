using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine.Text;
using CommandLine.Parsing;

namespace RepositorySplitter
{
    public class Options
    {
        [Option(HelpText="Whether to recursively search repository directories")]
        public bool Recursive { get; set; }

        [Option("repo", Required = true, HelpText = "Directory path for the repository to split.")]
        public string Repository { get; set; }

        [Option("srepo", Required = true, HelpText = "The name of the new repository that results from the splitting.")]
        public string SplitRepositoryName { get; set; }

        [Option(Required = true, HelpText = "Names of directories to include in the new repository, separated by a space. Directories are expected to be relative paths from the repository root.")]
        public string Directories { get; set; }

        /// <summary>
        /// Gets the usage for repository splitter options.
        /// </summary>
        /// <returns>The help text for repository splitter option usage.</returns>
        public string GetUsage()
        {
            return CommandLine.Text.HelpText.AutoBuild(this);
        }
    }
}
