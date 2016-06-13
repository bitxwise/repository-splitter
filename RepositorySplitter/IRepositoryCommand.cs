namespace RepositorySplitter
{
    /// <summary>
    /// Represents an executable revision control repository command.
    /// </summary>
    public interface IRepositoryCommand
    {
        /// <summary>
        /// Executes the represented command with the specified arguments.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        void Execute(string args);

        /// <summary>
        /// Gets or sets the working directory for the represented command.
        /// </summary>
        string WorkingDirectory { get; set; }
    }
}