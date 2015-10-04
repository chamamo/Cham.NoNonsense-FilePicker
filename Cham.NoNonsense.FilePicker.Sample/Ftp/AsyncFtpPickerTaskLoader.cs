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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Dropbox.CoreApi.Android;
using Dropbox.CoreApi.Android.Exception;
using File = Java.IO.File;
using IOException = Java.IO.IOException;
using Object = Java.Lang.Object;

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{

    public class AsyncFtpPickerTaskLoader : AbstractAsyncTaskLoader<FtpFile>
    {
        protected FilePickerMode Mode;

        protected bool LoggedIn;
        protected string Server;
        protected int Port;
        protected FtpClient Ftp;
        protected string UserName;
        protected string Password;

        public AsyncFtpPickerTaskLoader(Activity activity, Func<FtpFile, bool> isItemVisible, FtpFile root,
            FtpFile currentPath, FilePickerMode mode, bool loggedIn, string server, int port, FtpClient ftp,
            string userName, string password)
            : base(activity, isItemVisible, root, currentPath)
        {
            Mode = mode;
            LoggedIn = loggedIn;
            Server = server;
            Port = port;
            Ftp = ftp;
            UserName = userName;
            Password = password;
        }

        protected override IEnumerable<FtpFile> Load()
        {



            var files = new List<FtpFile>();
            try
            {
                // handle if directory does not exist. Fall back to root.
                if (CurrentPath == null || !CurrentPath.IsDirectory)
                {
                    CurrentPath = Root;
                }

                files.AddRange(Ftp.ListFiles(CurrentPath).Where(f => IsItemVisible(f)));
            }
            catch (IOException e)
            {
                Log.Error("", "IOException: " + e.Message);
            }

            return files.OrderBy(f => !f.IsDirectory).ThenBy(f => f.Name.ToLower());
        }

        protected override void OnStartLoading()
        {
            base.OnStartLoading();
            if (CurrentPath == null || !CurrentPath.IsDirectory)
            {
                CurrentPath = Root;
            }
            ForceLoad();
        }
    }
}