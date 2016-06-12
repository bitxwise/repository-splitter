using System;
using System.Diagnostics;

namespace RepositorySplitter
{
    /// <summary>
    /// Represents an executable git command that assumes git is available in a PATH environment variable.
    /// </summary>
    /// <remarks>
    /// This approach to executing git operations may be a bit ugly. Consider refactoring later.
    /// </remarks>
    public class GitCommand
    {
        /// <summary>
        /// Name of the git executable.
        /// </summary>
        public const string GIT_EXECUTABLE_NAME = "git.exe";

        /// <summary>
        /// Raised when a new result message is available from git command execution.
        /// </summary>
        public event EventHandler<ResultMessageEventArgs> NewResultMessage;

        /// <summary>
        /// Gets or sets the working directory for the Git command.
        /// </summary>
        public string WorkingDirectory { get; set; }

        /// <summary>
        /// Initializes a new instance of the RepositorySplitter.GitCommand class.
        /// </summary>
        public GitCommand()
        {
            WorkingDirectory = Environment.CurrentDirectory;
        }

        /// <summary>
        /// Initializes a new instance of the RepositorySplitter.GitCommand class with the specified working directory.
        /// </summary>
        /// <param name="workingDirectory">The working directory for the git command.</param>
        public GitCommand(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        /// <summary>
        /// Executes git with the specified arguments.
        /// </summary>
        /// <param name="args">Git arguments.</param>
        public void Execute(string args)
        {
            using(Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = GIT_EXECUTABLE_NAME,
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    WorkingDirectory = this.WorkingDirectory
                };

                // execute git command and broadcast result messages
                process.Start();
                while (!process.StandardOutput.EndOfStream)
                {
                    OnNewResultMessage(process.StandardOutput.ReadLine());
                }
            };
        }

        /// <summary>
        /// Raises the NewResultMessage event with the specified output.
        /// </summary>
        /// <param name="message">The new result message.</param>
        protected virtual void OnNewResultMessage(string message)
        {
            var e = NewResultMessage;
            if (e != null) e(this, new ResultMessageEventArgs(message));
        }
    }
}
