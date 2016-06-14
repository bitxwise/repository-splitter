using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;

namespace RepositorySplitter.Tests
{
    [TestClass()]
    public class DirectoryTests
    {
        /// <summary>
        /// A test to verify that the determination of which directories to remove correctly includes directories
        /// that are not flagged for retainment, and also excludes git directories. Parent directories of subdirectories
        /// to retain should also be excluded from removal.
        /// </summary>
        [TestMethod(), TestCategory("Directory Tests")]
        public void GetDirectoriesToRemoveTest()
        {
            /***** ARRANGE *****/
            var directoryHelper = Substitute.For<IDirectoryHelper>();
            var target = new SpecifiedDirectoriesSplitStrategy(Substitute.For<IRepositoryCommand>(), directoryHelper);
            target.IncludeSubdirectories = true;

            string newRepository = "newRepo";

            var directoriesToRetain = new string[] {
                // intentionally out of order
                "abc/def/ghi",
                "foo/bar",
                "abc/def/xyz"
            };

            var expected = new string[] {
                "abc/def/wtf",
                "db",
                "foo/blah"
            };

            var inexplicitParentDirectories = new string[] {
                "abc",
                "foo"
            };

            var gitDirectories = new string[] {
                ".git",
                ".git/hooks",
                ".git/logs",
                ".git/objects",
                ".git/refs"
            };

            target.DirectoriesToRetain = directoriesToRetain;
            
            directoryHelper.GetDirectories(newRepository, "*", SearchOption.AllDirectories).Returns(
                // assume directories are returned in order of hierarchy, then by alphabetical order
                directoriesToRetain
                    .Concat(expected)
                    .Concat(gitDirectories)
                    .Concat(inexplicitParentDirectories)
                    .OrderBy(d => d.Count(di => di == '/')) // order by hierarchy
                    .ThenBy(d => d)                         // then by alphabetical order
            );

            /***** ACT *****/
            var actual = target.GetDirectoriesToRemove(newRepository);

            /***** ASSERT *****/
            CollectionAssert.AreEquivalent(expected.ToArray(), actual.ToArray());
        }
    }
}
