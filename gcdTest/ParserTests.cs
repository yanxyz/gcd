using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gcd;

namespace gcdTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void SSH()
        {
            TestSSH("git@github.com:yanxyz/test.git");
            TestSSH("git@git.coding.net:yanxyz/test.git");
        }

        public void TestSSH(string url)
        {
            var repo = new Parser(url);
            Assert.AreEqual("yanxyz-test", repo.Dir, false);
            Assert.IsNull(repo.Branch);
            Assert.AreEqual(url, repo.Url, false);
        }

        [TestMethod]
        public void GithubHTTPS()
        {
            TestHTTPS("https://github.com/yanxyz/test.git");
        }

        public void TestHTTPS(string url)
        {
            var repo = new Parser(url);
            Assert.AreEqual("yanxyz-test", repo.Dir, false);
            Assert.IsNull(repo.Branch);
            Assert.AreEqual(url, repo.Url, false);
        }

        [TestMethod]
        public void GithubRepoIndexUrl()
        {
            TestRepoIndexUrl(
                "https://coding.net/u/yanxyz/p/test/git",
                "https://git.coding.net/yanxyz/test"
                );
        }

        public void TestRepoIndexUrl(string url, string repoUrl)
        {
            var repo = new Parser(url);
            Assert.AreEqual("yanxyz-test", repo.Dir, false);
            Assert.AreEqual(repoUrl, repo.Url, false);
        }

        [TestMethod]
        public void GithubBlobUrl()
        {
            TestUrl(
                "https://github.com/yanxyz/test/blob/foo/bar/README.md",
                "https://github.com/yanxyz/test"
                );
        }

        [TestMethod]
        public void GithubTreeUrl()
        {
            TestUrl(
                "https://github.com/yanxyz/test/tree/foo/bar/tests",
                "https://github.com/yanxyz/test"
                );
        }

        public void TestUrl(string url, string repoUrl)
        {
            // /blob/token/path
            // /tree/token/path
            // token 可能是 branch, tag, hash
            // branch, tag 的名字可能包含 "/"
            // hash 不能 git clone

            var repo = new Parser(url);
            Assert.AreEqual("yanxyz-test", repo.Dir, false);
            // 正确的应为 "foo/bar", 但是按简单情况考虑。
            Assert.AreEqual("foo", repo.Branch, false);
            Assert.AreEqual(repoUrl, repo.Url, false);
        }

        [TestMethod]
        public void GithubRepoNameSameAsUserName()
        {
            TestUrl2(
                "https://github.com/yanxyz/yanxyz/blob/master/README.md",
                "https://github.com/yanxyz/yanxyz"
                );
        }

        public void TestUrl2(string url, string repoUrl)
        {
            var repo = new Parser(url);
            Assert.AreEqual("yanxyz-yanxyz", repo.Dir, false);
            Assert.AreEqual("master", repo.Branch, false);
            Assert.AreEqual(repoUrl, repo.Url, false);
        }

        /**
         * coding.net
         */

        [TestMethod]
        public void CodingHTTPS()
        {
            TestHTTPS("https://git.coding.net/yanxyz/test.git");
        }

        [TestMethod]
        public void CodingRepoIndexUrl()
        {
            TestRepoIndexUrl(
                "https://coding.net/u/yanxyz/p/test/git",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [TestMethod]
        public void CodingBlobUrl()
        {
            TestUrl(
                "https://coding.net/u/yanxyz/p/test/git/blob/foo/bar/README.md",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [TestMethod]
        public void CodingTreeUrl()
        {
            TestUrl(
                "https://coding.net/u/yanxyz/p/test/git/tree/foo/bar/tests",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [TestMethod]
        public void CodingRepoNameSameAsUserName()
        {
            TestUrl2(
                "https://coding.net/u/yanxyz/p/yanxyz/git/blob/master/README.md",
                "https://git.coding.net/yanxyz/yanxyz"
                );
        }
    }
}
