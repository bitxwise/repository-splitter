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
        [TestMethod(), TestCategory("Directory Tests")]
        public void DirectoryRecursionTest()
        {
            /***** ARRANGE *****/
            var directoryHelper = Substitute.For<IDirectoryHelper>();
            var target = new SpecifiedDirectoriesSplitStrategy(Substitute.For<IRepositoryCommand>(), directoryHelper);

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

            target.Directories = directoriesToRetain;
            
            directoryHelper.GetDirectories(newRepository, "*", SearchOption.AllDirectories).Returns(
                // assume directories are returned in alphabetical order
                directoriesToRetain.Concat(expected).OrderBy(d => d)
            );

            /***** ACT *****/
            var actual = target.GetDirectoriesToRemove(newRepository);

            /***** ASSERT *****/
            Assert.AreEqual(expected, actual);
        }
    }
}
