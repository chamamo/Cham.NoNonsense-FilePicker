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
using Android.Views;
using Android.Widget;
using Dropbox.CoreApi.Android;
using Dropbox.CoreApi.Android.Exception;
using Java.IO;
using Object = Java.Lang.Object;

namespace Cham.NoNonsense.FilePicker.Sample
{
    public class AsyncMultimediaPickerTaskLoader : AbstractAsyncTaskLoader<DropboxApi.Entry>
    {
        protected DropboxApi DbApi;
        
        protected FilePickerMode Mode;

        public AsyncMultimediaPickerTaskLoader(Activity activity, Func<DropboxApi.Entry, bool> isItemVisible, DropboxApi dbApi, DropboxApi.Entry root, DropboxApi.Entry currentPath, FilePickerMode mode)
            : base(activity, isItemVisible, root, currentPath)
        {
            DbApi = dbApi;
            Mode = mode;
        }

        protected override IEnumerable<DropboxApi.Entry> Load()
        {

            var files = new List<DropboxApi.Entry>();
            try
            {

                if (!DbApi.Metadata(CurrentPath.Path, 1, null, false, null).IsDir)
                {
                    CurrentPath = Root;
                }

                var dirEntry = DbApi.Metadata(CurrentPath.Path, 0, null, true, null);


                files.AddRange(dirEntry.Contents.Cast<DropboxApi.Entry>().Where(entry => (Mode == FilePickerMode.File || Mode == FilePickerMode.Dir) || entry.IsDir));
            }
            catch (DropboxException ignored)
            {
            }

            return files.OrderBy(f => !f.IsDir).ThenBy(f => f.FileName().ToLower());
        }

        protected override void OnStartLoading()
        {
            base.OnStartLoading();
            if (CurrentPath == null || !CurrentPath.IsDir)
            {
                CurrentPath = Root;
            }
            ForceLoad();
        }
    }
}