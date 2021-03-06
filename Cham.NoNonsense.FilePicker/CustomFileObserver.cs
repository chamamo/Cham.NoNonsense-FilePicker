// Copyright (c) 2015 Mourad Chama
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// dzdz
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Cham.NoNonsense.FilePicker
{
    internal class CustomFileObserver : FileObserver
    {
        public EventHandler<string> Event;

        public CustomFileObserver(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public CustomFileObserver(string path)
            : base(path)
        {
        }

        public CustomFileObserver(string path, FileObserverEvents mask)
            : base(path, mask)
        {
        }

        public override void OnEvent(FileObserverEvents e, string path)
        {
            if (Event != null) Event(this, path);
        }
    }
}