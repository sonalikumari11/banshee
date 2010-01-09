//
// DatabaseAlbumInfoTests.cs
//
// Author:
//   John Millikin <jmillikin@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#if ENABLE_TESTS

using NUnit.Framework;
using Banshee.Collection.Database;
using Banshee.Collection;

namespace Banshee.Collection.Database.Tests
{
    [TestFixture]
    public class DatabaseAlbumInfoTests
    {
        static DatabaseAlbumInfoTests () {
            Banshee.Database.BansheeDatabaseSettings.CheckTables = false;
        }

        protected void AssertTitleSort (string title, string title_sort, byte[] expected)
        {
            DatabaseAlbumInfo info = new DatabaseAlbumInfo ();
            info.Title = title;
            info.TitleSort = title_sort;
            Assert.AreEqual (expected, info.TitleSortKey);
        }

        protected void AssertTitleLowered (string title, string expected)
        {
            DatabaseAlbumInfo info = new DatabaseAlbumInfo ();
            info.Title = title;
            Assert.AreEqual (expected, info.TitleLowered);
        }

        protected void AssertArtistNameSort (string name, string name_sort, byte[] expected)
        {
            DatabaseAlbumInfo info = new DatabaseAlbumInfo ();
            info.ArtistName = name;
            info.ArtistNameSort = name_sort;
            Assert.AreEqual (expected, info.ArtistNameSortKey);
        }

        protected void AssertArtistNameLowered (string name, string expected)
        {
            DatabaseAlbumInfo info = new DatabaseAlbumInfo ();
            info.ArtistName = name;
            Assert.AreEqual (expected, info.ArtistNameLowered);
        }

        [Test]
        public void TestWithoutTitleSortKey ()
        {
            AssertTitleSort ("", null,  Hyena.StringUtil.SortKey (AlbumInfo.UnknownAlbumTitle));
            AssertTitleSort ("a", null, new byte[] {14, 2, 1, 1, 1, 1, 0});
            AssertTitleSort ("a", "",   new byte[] {14, 2, 1, 1, 1, 1, 0});
            AssertTitleSort ("A", null, new byte[] {14, 2, 1, 1, 1, 1, 0});
        }

        [Test]
        public void TestTitleSortKey ()
        {
            AssertTitleSort ("Title", "a", new byte[] {14, 2, 1, 1, 1, 1, 0});
            AssertTitleSort ("Title", "A", new byte[] {14, 2, 1, 1, 1, 1, 0});
        }

        [Test]
        public void TestTitleLowered ()
        {
            AssertTitleLowered ("", AlbumInfo.UnknownAlbumTitle.ToLower ());
            AssertTitleLowered ("A", "a");
            AssertTitleLowered ("\u0104", "a");
        }

        [Test]
        public void TestWithoutArtistNameSortKey ()
        {
            AssertArtistNameSort ("", null, Hyena.StringUtil.SortKey (ArtistInfo.UnknownArtistName));
            AssertArtistNameSort ("a", null, new byte[] {14, 2, 1, 1, 1, 1, 0});
            AssertArtistNameSort ("A", null, new byte[] {14, 2, 1, 1, 1, 1, 0});

            AssertArtistNameSort ("a", "", new byte[] {14, 2, 1, 1, 1, 1, 0});
        }

        [Test]
        public void TestArtistNameSortKey ()
        {
            AssertArtistNameSort ("Title", "a", new byte[] {14, 2, 1, 1, 1, 1, 0});
            AssertArtistNameSort ("Title", "A", new byte[] {14, 2, 1, 1, 1, 1, 0});
        }

        [Test]
        public void TestArtistNameLowered ()
        {
            AssertArtistNameLowered ("", ArtistInfo.UnknownArtistName.ToLower ());
            AssertArtistNameLowered ("A", "a");
            AssertArtistNameLowered ("\u0104", "a");
        }
    }
}

#endif
