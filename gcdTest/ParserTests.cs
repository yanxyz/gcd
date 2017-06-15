using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using gcd;

namespace gcdTest
{
    public class ParserTests
    {
        [Fact]
        public void SSH()
        {
            TestSSH("git@github.com:yanxyz/test.git");
            TestSSH("git@git.coding.net:yanxyz/test.git");
        }

        public void TestSSH(string url)
        {
            var repo = new Parser(url);
            Assert.Equal("yanxyz-test", repo.Dir, false);
            Assert.Null(repo.Branch);
            Assert.Equal(url, repo.Url, false);
        }

        [Fact]
        public void GithubHTTPS()
        {
            TestHTTPS("https://github.com/yanxyz/test.git");
        }

        public void TestHTTPS(string url)
        {
            var repo = new Parser(url);
            Assert.Equal("yanxyz-test", repo.Dir, false);
            Assert.Null(repo.Branch);
            Assert.Equal(url, repo.Url, false);
        }

        [Fact]
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
            Assert.Equal("yanxyz-test", repo.Dir, false);
            Assert.Equal(repoUrl, repo.Url, false);
        }

        [Fact]
        public void GithubBlobUrl()
        {
            TestUrl(
                "https://github.com/yanxyz/test/blob/foo/bar/README.md",
                "https://github.com/yanxyz/test"
                );
        }

        [Fact]
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
            Assert.Equal("yanxyz-test", repo.Dir, false);
            // 正确的应为 "foo/bar", 但是按简单情况考虑。
            Assert.Equal("foo", repo.Branch, false);
            Assert.Equal(repoUrl, repo.Url, false);
        }

        [Fact]
        public void GithubTreeSha()
        {
            Assert.Throws<Exception>(() => new Parser("https://github.com/yanxyz/test/tree/9c739b913fd9553d2db1621dbfe21e41d2add732"));
        }

        [Fact]
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
            Assert.Equal("yanxyz-yanxyz", repo.Dir, false);
            Assert.Equal("master", repo.Branch, false);
            Assert.Equal(repoUrl, repo.Url, false);
        }

        /**
         * coding.net
         */

        [Fact]
        public void CodingHTTPS()
        {
            TestHTTPS("https://git.coding.net/yanxyz/test.git");
        }

        [Fact]
        public void CodingRepoIndexUrl()
        {
            TestRepoIndexUrl(
                "https://coding.net/u/yanxyz/p/test/git",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [Fact]
        public void CodingBlobUrl()
        {
            TestUrl(
                "https://coding.net/u/yanxyz/p/test/git/blob/foo/bar/README.md",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [Fact]
        public void CodingTreeUrl()
        {
            TestUrl(
                "https://coding.net/u/yanxyz/p/test/git/tree/foo/bar/tests",
                "https://git.coding.net/yanxyz/test"
                );
        }

        [Fact]
        public void CodingRepoNameSameAsUserName()
        {
            TestUrl2(
                "https://coding.net/u/yanxyz/p/yanxyz/git/blob/master/README.md",
                "https://git.coding.net/yanxyz/yanxyz"
                );
        }

    }
}
