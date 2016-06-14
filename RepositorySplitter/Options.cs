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
        [Option('r', Required = true, HelpText = "Directory path for the repository to split.")]
        public string Repository { get; set; }

        [Option('s', Required = true, HelpText = "The name of the new repository that results from the splitting.")]
        public string SplitRepositoryName { get; set; }

        [Option('d', Required = true, HelpText = "Names of directories to include in the new repository, separated by a space. Directories are expected to be relative paths from the repository root.")]
        public string Directories { get; set; }

        [Option("subdirectories", HelpText = "Whether the directories to include are at the subdirectory level.")]
        public bool IncludeSubdirectories { get; set; }

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
