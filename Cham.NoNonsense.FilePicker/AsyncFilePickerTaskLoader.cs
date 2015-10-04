//
// Copyright (c) 2015 Mourad Chama
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;
using AsyncTaskLoader = Android.Support.V4.Content.AsyncTaskLoader;
using Object = Java.Lang.Object;

namespace Cham.NoNonsense.FilePicker
{
    public class AsyncFilePickerTaskLoader : AbstractAsyncTaskLoader<File>
    {
        internal CustomFileObserver FileObserver;

        public AsyncFilePickerTaskLoader(Activity activity, File currentPath, File root, Func<File, bool> isItemVisible)
            : base(activity,isItemVisible, root, currentPath)
        {
        }

        protected override IEnumerable<File> Load()
        {
            var listFiles = CurrentPath.ListFiles().AsEnumerable();
            listFiles = listFiles.Where(f => IsItemVisible(f)).OrderBy(f => f.IsFile).ThenBy(f => f.AbsolutePath);
            return listFiles;
        }

        protected override void OnStartLoading()
        {
            base.OnStartLoading();

            // handle if directory does not exist. Fall back to root.
            if (CurrentPath == null || !CurrentPath.IsDirectory)
            {
                CurrentPath = Root;
            }

            // Start watching for changes
            FileObserver = new CustomFileObserver(CurrentPath.Path,
                FileObserverEvents.Create | FileObserverEvents.Delete | FileObserverEvents.MovedFrom | FileObserverEvents.MovedTo);
            FileObserver.Event += (s, e) =>
            {
                OnContentChanged();
            };
            FileObserver.StartWatching();

            ForceLoad();
        }

        protected override void OnReset()
        {
            base.OnReset();

            // Stop watching
            if (FileObserver != null)
            {
                FileObserver.StopWatching();
                FileObserver = null;
            }
        }
    }
}