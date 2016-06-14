using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RepositorySplitter
{
    /// <summary>
    /// A repository splitting strategy that creates a new repository containing only specified directories.
    /// </summary>
    public class SpecifiedDirectoriesSplitStrategy : ISplitStrategy
    {
        private readonly IDirectoryHelper _DirectoryHelper;
        /// <summary>
        /// Gets the helper used for directory operations.
        /// </summary>
        public IDirectoryHelper DirectoryHelper
        {
            get { return _DirectoryHelper; }
        }

        /// <summary>
        /// Gets or sets names of directories to include in the new repository.
        /// </summary>
        public IEnumerable<string> DirectoriesToRetain { get; set; }

        private readonly IRepositoryCommand _Git;
        /// <summary>
        /// Gets the git command used to perform git operations.
        /// </summary>
        /// <remarks>
        /// Assumes git command will not be altered elsewhere. This may be a poor assumption later but is acceptable for now.
        /// </remarks>
        public IRepositoryCommand Git
        {
            get { return _Git; }
        }

        /// <summary>
        /// Whether to include subdirectories when filtering content and history.
        /// </summary>
        public bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// Gets or sets the name of the new repository that results from the splitting.
        /// </summary>
        public string SplitRepositoryName { get; set; }

        /// <summary>
        /// Initializes a new instance of the RepositorySplitter.SpecifiedDirectoriesSplitStrategy class with the specified repository path  and git command.
        /// </summary>
        /// <param name="git">The git command with which to perform git operations.</param>
        /// <param name="directoryHelper">The helper used for directory operations.</param>
        public SpecifiedDirectoriesSplitStrategy(IRepositoryCommand git, IDirectoryHelper directoryHelper)
        {
            _Git = git;
            _DirectoryHelper = directoryHelper;
        }

        /// <summary>
        /// Splits the specified repository by creating a new repository containing only specified directories.
        /// </summary>
        /// <param name="repository">Directory path for the repository to split.</param>
        public void Split(string repository)
        {
            #region Pre-Verifications
            // TODO: Consider moving these into directory helper
            // verify repository directory exists
            if (!Directory.Exists(repository))
                throw new DirectoryNotFoundException(repository);

            // verify we have a name for the new repository
            if (string.IsNullOrWhiteSpace(SplitRepositoryName))
                throw new Exception("The new repository name is invalid.");

            // verify we know which directories to split into the new repository
            if (DirectoriesToRetain == null || DirectoriesToRetain.Count() == 0)
                throw new Exception("No directories will be included in the new repository.");

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

            // identify directories to remove
            var directoriesToRemove = GetDirectoriesToRemove(newRepository);

            // remove all directories except for the the directories to include
            // TODO: Consider offering alternative filter-branch options (e.g. besides index-filter)
            Git.Execute(string.Format("filter-branch --index-filter \"git rm -r --cached --ignore-unmatch {0}\" --prune-empty -f -- --all", directoriesToRemove.Implode(" ")));
            Git.Execute("for-each-ref --format=\" % (refname)\" refs/original/ | xargs -n 1 git update-ref -d");
            
            // add each removed subdirectory into .gitignore
            // TODO: Ensure each directory is on its own line in .gitignore
            File.AppendAllLines(string.Format("{0}\\.gitignore", newRepository), directoriesToRemove.Select(sdr => DirectoryHelper.AppendDirectorySeparatorChar(sdr, true)));

            // commit removal and invoke git garbage collection
            // TODO: Support custom commit message
            Git.Execute("commit -a -m \"Removed unnecessary directories from git history and added them to .gitignore\"");
            Git.Execute("gc");

            // TODO: Support removal of directories that were split into new repository from original repository
        }

        /// <summary>
        /// Gets the directories, including subdirectories if IncludeSubdirectories is true, to remove from the new repository.
        /// </summary>
        /// <param name="newRepository">The path to the new repository.</param>
        /// <returns></returns>
        public IEnumerable<string> GetDirectoriesToRemove(string newRepository)
        {
            // get all directories for removal, and exclude git directories, as well as directories (and their subdirectories) to retain
            var directories = new List<string>(DirectoryHelper.GetDirectories(newRepository, "*", true ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            directories.RemoveAll(d => d.StartsWith(".git"));   // always retain git directories
            directories.RemoveAll(d => DirectoriesToRetain.Any(dtr => d == dtr || d.StartsWith(dtr + Path.AltDirectorySeparatorChar)));

            // ensure parent directories for directories to retain are also retained
            // TODO: Refactor this into a directory helper
            foreach (var directory in DirectoriesToRetain)
            {
                string path;
                int index = directory.LastIndexOf(Path.AltDirectorySeparatorChar);
                while (index > 0)
                {
                    path = directory.Substring(0, index);
                    if (directories.Contains(path)) directories.Remove(path);
                    index = path.LastIndexOf(Path.AltDirectorySeparatorChar);
                }
            }
            
            // for all directories to remove, their subdirectories will automatically be removed,
            // so no need to duplicate the effort by including them in the filter-branch match
            // TODO: Refactor this into directory helper and for better performance
            bool keepGoing = true;
            List<string> alreadyScanned = new List<string>();
            var directoriesCopy = directories.ToList();
            while (keepGoing)
            {
                keepGoing = false;
                foreach (var dc in directoriesCopy)
                {
                    if (!DirectoriesToRetain.Any(dtr => dtr.StartsWith(dc)))
                    {
                        directories.RemoveAll(d => d.StartsWith(dc + Path.AltDirectorySeparatorChar));
                        alreadyScanned.Add(dc);
                        keepGoing = true;
                        break;
                    }
                }

                if (keepGoing)
                {
                    directoriesCopy = directories.ToList();
                    directoriesCopy.RemoveAll(d => alreadyScanned.Contains(d));
                }
            }
            
            return directories;
        }
    }
}