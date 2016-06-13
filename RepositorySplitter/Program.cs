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
            
            if(!Parser.Default.ParseArguments(args, options))
                return;
            
            GitCommand git = new GitCommand();
            git.NewResultMessage += (s, e) => {
                Console.WriteLine(e.ResultMessage);
            };

            // prepare split strategy
            // TODO: Use factory as new strategies are added
            SpecifiedDirectoriesSplitStrategy splitStrategy = new SpecifiedDirectoriesSplitStrategy(git);
            splitStrategy.SplitRepositoryName = options.SplitRepositoryName;
            splitStrategy.Directories = options.SubDirectories.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
