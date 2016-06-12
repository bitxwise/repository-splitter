using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RepositorySplitter
{
    /// <summary>
    /// A repository splitting strategy that creates a new repository containing only specified subdirectories.
    /// </summary>
    public class SpecifiedSubDirectoriesSplitStrategy : ISplitStrategy
    {
        private readonly GitCommand _Git;
        /// <summary>
        /// Gets the git command used to perform git operations.
        /// </summary>
        /// <remarks>
        /// Assumes git command will not be altered elsewhere. This may be a poor assumption later but is acceptable for now.
        /// </remarks>
        public GitCommand Git
        {
            get { return _Git; }
        }

        /// <summary>
        /// Gets or sets the name of the new repository that results from the splitting.
        /// </summary>
        public string SplitRepositoryName { get; set; }

        /// <summary>
        /// Gets or sets names of subdirectories to include in the new repository.
        /// </summary>
        public IEnumerable<string> SubDirectories { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the RepositorySplitter.SpecifiedSubDirectoriesSplitStrategy class with the specified repository path  and git command.
        /// </summary>
        /// <param name="git">The git command with which to perform git operations.</param>
        public SpecifiedSubDirectoriesSplitStrategy(GitCommand git)
        {
            _Git = git;
        }

        /// <summary>
        /// Splits the specified repository by creating a new repository containing only specified subdirectories.
        /// </summary>
        /// <param name="repository">Directory path for the repository to split.</param>
        public void Split(string repository)
        {
            #region Pre-Verifications
            // verify repository directory exists
            if (!Directory.Exists(repository))
                throw new DirectoryNotFoundException(repository);

            // verify we have a name for the new repository
            if (string.IsNullOrWhiteSpace(SplitRepositoryName))
                throw new Exception("The new repository name is invalid.");

            // verify we know which subdirectories to split into the new repository
            if (SubDirectories == null || SubDirectories.Count() == 0)
                throw new Exception("No subdirectories will be included in the new repository.");

            string parentDirectory = Directory.GetParent(repository).FullName;
            string newRepository = string.Format("{0}\\{1}", parentDirectory, SplitRepositoryName);
            
            // verify new repository directory does not already exist
            if (Directory.Exists(newRepository))
                throw new Exception("The new repository directory already exists.");
            #endregion Pre-Verifications

            // clone existing repository into to-be new repository
            Git.WorkingDirectory = parentDirectory;
            Git.Execute(string.Format("clone {0} {1}", repository, newRepository));

            // protect existing repository from changes
            Git.WorkingDirectory = newRepository;
            Git.Execute("remote rm origin");

            // identify subdirectories to remove
            var allSubDirectories = Directory.GetDirectories(newRepository)
                .Select(d => DirectoryHelper.GetRelativePath(newRepository, d).TrimEnd(Path.DirectorySeparatorChar));

            List<string> subDirectoriesToRemove = new List<String>(allSubDirectories);
            foreach(string subdirectory in SubDirectories)
                subDirectoriesToRemove.Remove(subdirectory);
            
            // do not allow removal of .git folder
            subDirectoriesToRemove.Remove(".git");

            // remove all subdirectories except for the the subdirectories to include
            // TODO: Consider offering alternative filter-branch options (e.g. besides index-filter)
            Git.Execute(string.Format("filter-branch --index-filter \"git rm -r --cached --ignore-unmatch {0}\" --prune-empty -f -- --all", subDirectoriesToRemove.Implode(" ")));
            Git.Execute("for-each-ref --format=\" % (refname)\" refs/original/ | xargs -n 1 git update-ref -d");
            
            // add each removed subdirectory into .gitignore
            // TODO: Ensure each directory is on its own line in .gitignore
            File.AppendAllLines(string.Format("{0}\\.gitignore", newRepository), subDirectoriesToRemove.Select(sdr => DirectoryHelper.AppendDirectorySeparatorChar(sdr, true)));

            // commit removal and invoke git garbage collection
            // TODO: Support custom commit message
            Git.Execute("commit -a -m \"Removed unnecessary directories from git history and added them to .gitignore\"");
            Git.Execute("gc");

            // TODO: Support removal of directories that were split into new repository from original repository
        }
    }
}