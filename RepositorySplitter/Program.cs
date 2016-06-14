using CommandLine;
using System;
using System.Linq;

namespace RepositorySplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            // parse specified arguments
            var options = new Options();
            
            if(!Parser.Default.ParseArgumentsStrict(args, options))
                return;
            
            GitCommand git = new GitCommand();
            git.NewResultMessage += (s, e) => {
                Console.WriteLine(e.ResultMessage);
            };

            WindowsDirectoryHelper directoryHelper = new WindowsDirectoryHelper();

            // prepare split strategy
            // TODO: Use factory as new strategies are added
            SpecifiedDirectoriesSplitStrategy splitStrategy = new SpecifiedDirectoriesSplitStrategy(git, directoryHelper);
            
            splitStrategy.SplitRepositoryName = options.SplitRepositoryName;
            splitStrategy.DirectoriesToRetain = options.Directories.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {
                // split repository
                splitStrategy.Split(options.Repository);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
