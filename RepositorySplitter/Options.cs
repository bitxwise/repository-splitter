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
        [Option('r', "repo", Required = true, HelpText = "Directory path for the repository to split.")]
        public string Repository { get; set; }

        [Option('s', "srepo", Required = true, HelpText = "The name of the new repository that results from the splitting.")]
        public string SplitRepositoryName { get; set; }

        [Option('d', "dir", Required = true, HelpText = "Names of subdirectories to include in the new repository, separated by a space.")]
        public string SubDirectories { get; set; }

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
