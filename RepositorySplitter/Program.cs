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

            // prepare split strategy
            // TODO: Use factory as new strategies are added
            SpecifiedSubDirectoriesSplitStrategy splitStrategy = new SpecifiedSubDirectoriesSplitStrategy(git);
            splitStrategy.SplitRepositoryName = options.SplitRepositoryName;
            splitStrategy.SubDirectories = options.SubDirectories.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

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
