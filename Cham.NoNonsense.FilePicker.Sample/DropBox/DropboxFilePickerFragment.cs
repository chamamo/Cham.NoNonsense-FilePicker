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

using System.Threading.Tasks;
using Android.Net;
using Android.OS;
using Android.Support.V4.Content;
using Android.Text;
using Android.Widget;
using Dropbox.CoreApi.Android;
using Dropbox.CoreApi.Android.Exception;
using Java.IO;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker.Sample.DropBox
{
    public class DropboxFilePickerFragment : AbstractFilePickerFragment<DropboxApi.Entry> {

    private DropboxApi dbApi;

        public DropboxFilePickerFragment(DropboxApi api)
        {
            if (api == null)
            {
                throw new NullPointerException("FileSystem may not be null");
            }
            else if (!api.Session.IsLinked)
            {
                throw new IllegalArgumentException("Must be linked with Dropbox");
            }

            this.dbApi = api;
        }

        protected override void OnNewFodler(string name)
        {
            var folder = new File(CurrentPath.Path, name);
            Task.Run(() =>
            {
                try
                {
                    dbApi.CreateFolder(folder.Path);
                    CurrentPath = dbApi.Metadata(folder.Path, 1, null, false, null);
                    Refresh();
                }
                catch (DropboxException e)
                {
                    Toast.MakeText(Activity, Resource.String.nnf_create_folder_error, ToastLength.Short).Show();
                }
            });
        }


        public override bool IsDir(DropboxApi.Entry file)
        {
            return file.IsDir;
        }

        public override DropboxApi.Entry GetParent(DropboxApi.Entry from) {
        // Take care of a slight limitation in Dropbox code:
        if (from.Path.Length > 1 && from.Path.EndsWith("/")) {
            from.Path = from.Path.Substring(0, from.Path.Length - 1);
        }
        var parent = from.ParentPath();
        if (TextUtils.IsEmpty(parent)) {
            parent = "/";
        }

        return GetPath(parent);

    }

        public override DropboxApi.Entry GetPath(string path)
        {
            return new DropboxApi.Entry {Path = path, IsDir = true};
        }

        public override string GetFullPath(DropboxApi.Entry file)
        {
            return file.Path;
        }

        public override string GetName(DropboxApi.Entry file)
        {
            return file.FileName();
        }

        public override DropboxApi.Entry Root
        {
            get { return GetPath("/"); }
        }

        public override Uri ToUri(DropboxApi.Entry file)
        {
            return new Uri.Builder().Scheme("dropbox").Authority("").Path(file.Path).Build();
        }

        public override AbstractAsyncTaskLoader<DropboxApi.Entry> Loader
        {
            get { return new AsyncDropBoxPickerTaskLoader(Activity, (s) => true, dbApi, Root,CurrentPath, Mode); }
        }
}

}