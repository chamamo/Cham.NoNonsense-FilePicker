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
using System.Runtime.InteropServices;
using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Widget;
using Java.Lang;
using Java.IO;
using String = Java.Lang.String;
using Android.Net;
using Android.Support.V7.Widget;
using Android.Support.V7.Widget.Util;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace Cham.NoNonsense.FilePicker
{
    public class FilePickerFragment : AbstractFilePickerFragment<File>
    {

        protected static readonly int PermissionsRequestWriteExternalStorage = 1;

        protected override bool HasPermission
        {
            get
            {
                if ((int)Build.VERSION.SdkInt < 23)
                    return true;
                return (int)Permission.Granted ==
                ContextCompat.CheckSelfPermission(Context, Manifest.Permission.WriteExternalStorage);
            }
        }

        public override File Root
        {
            get { return new File("/"); }

        }

        public override AbstractAsyncTaskLoader<File> Loader
        {
            get { return new AsyncFilePickerTaskLoader(Activity, CurrentPath, Root, (File f) => IsDir(f) || (Mode == FilePickerMode.File || Mode == FilePickerMode.File)); }
        }

        protected override void HandlePermission()
        {
//         Should we show an explanation?
//        if (shouldShowRequestPermissionRationale(
//                Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
//             Explain to the user why we need permission
//        }

            RequestPermissions(new string[] {Manifest.Permission.WriteExternalStorage},
                PermissionsRequestWriteExternalStorage);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            // If arrays are empty, then process was cancelled
            if (permissions.Length == 0)
            {
                // Treat this as a cancel press
                if (Listener != null)
                {
                    Listener.OnCancelled();
                }
            }
            else
            {
                // if (requestCode == PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE) {
                if ((int) Permission.Granted == grantResults[0])
                {
                    // Do refresh
                    Refresh();
                }
                else
                {
                    Toast.MakeText(Context, Resource.String.nnf_permission_external_write_denied,
                        ToastLength.Short).Show();
                    // Treat this as a cancel press
                    if (Listener != null)
                    {
                        Listener.OnCancelled();
                    }
                }
            }
        }

        public override bool IsDir(File path)
        {
            return path.IsDirectory;
        }

        public override string GetName(File path)
        {
            return path.Name;
        }

        public override File GetParent(File from)
        {
            if (from.Path.Equals(Root.Path))
            {
                // Already at root, we can't go higher
                return from;
            }
            else if (from.ParentFile != null)
            {
                if (from.IsFile)
                {
                    return GetParent(from.ParentFile);
                }
                else
                {
                    return from.ParentFile;
                }
            }
            else
            {
                return from;
            }
        }

        public override File GetPath(string path)
        {
            return new File(path);
        }

        public override string GetFullPath(File path)
        {
            return path.Path;
        }

        public override Uri ToUri(File file)
        {
            return Uri.FromFile(file);
        }

        protected override void OnNewFodler(string name)
        {
            var folder = new File(CurrentPath, name);

            if (folder.Mkdir())
            {
                CurrentPath = folder;
                Refresh();
            }
            else
            {
                Toast.MakeText(Activity, Resource.String.nnf_create_folder_error,
                    ToastLength.Short).Show();
            }
        }

        protected int CompareFiles(Java.IO.File lhs, Java.IO.File rhs)
        {
            if (lhs.IsDirectory && !rhs.IsDirectory)
            {
                return -1;
            }
            else if (rhs.IsDirectory && !lhs.IsDirectory)
            {
                return 1;
            }
            else
            {
                return string.Compare(lhs.Name, rhs.Name, StringComparison.CurrentCultureIgnoreCase);
            }
        }
    }
}