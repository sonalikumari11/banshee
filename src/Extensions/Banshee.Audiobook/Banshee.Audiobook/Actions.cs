//
// Actions.cs
//
// Authors:
//   Gabriel Burt <gburt@novell.com>
//
// Copyright (C) 2009 Novell, Inc.
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

using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Unix;
using Gtk;

namespace Banshee.Audiobook
{
    public class Actions : Banshee.Gui.BansheeActionGroup
    {
        public Actions () : base ("Audiobook")
        {
            Add (
                new ActionEntry ("AudiobookBookPopup", null, null, null, null, (o, a) => {
                    ShowContextMenu ("/AudiobookBookPopup");
                }),
                new ActionEntry ("AudiobookOpen", null, Catalog.GetString ("Open Book"), null, null, (o, a) => {
                    Console.WriteLine ("open book!");
                })
            );

            /*AddImportant (
                new ActionEntry ("VisitInternetArchive", Stock.JumpTo, Catalog.GetString ("Visit Archive.org"), null, null, (o, a) => {
                    Banshee.Web.Browser.Open ("http://archive.org");
                })
            );*/

            AddUiFromFile ("GlobalUI.xml");

            Register ();
        }
    }
}