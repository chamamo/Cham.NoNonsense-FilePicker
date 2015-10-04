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
using Android.Util;
using Android.Widget;
using Java.IO;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{
    public class FtpPickerFragment : AbstractFilePickerFragment<FtpFile>
    {

        private static readonly string KEY_FTP_SERVER = "KEY_FTP_SERVER";
        private static readonly string KEY_FTP_PORT = "KEY_FTP_PORT";
        private static readonly string KEY_FTP_USERNAME = "KEY_FTP_USERNAME";
        private static readonly string KEY_FTP_PASSWORD = "KEY_FTP_PASSWORD";
        private static readonly string KEY_FTP_ROOTDIR = "KEY_FTP_ROOTDIR";
        private static readonly string TAG = "NoNonsenseFtp";
        private FtpClient ftp;
        private string server;
        private int port;
        private string username;
        private string password;
        private bool loggedIn = false;
        private string rootDir = "/";

        public FtpPickerFragment()
        {
            
        }

        public static AbstractFilePickerFragment<FtpFile> newInstance(string startPath, FilePickerMode mode,
            bool allowMultiple, bool allowCreateDir, string server, int port, string username, string password,
            string rootDir)
        {
            var fragment = new FtpPickerFragment();
            // Add arguments
            fragment.SetArgs(startPath, mode, allowMultiple, allowCreateDir);
            var args = fragment.Arguments;

            // Add ftp related stuff
            args.PutString(KEY_FTP_ROOTDIR, rootDir);
            args.PutString(KEY_FTP_SERVER, server);
            args.PutInt(KEY_FTP_PORT, port);
            if (username != null && password != null)
            {
                args.PutString(KEY_FTP_USERNAME, username);
                args.PutString(KEY_FTP_PASSWORD, password);
            }

            return fragment;
        }

        public override void OnCreate(Bundle b)
        {
            base.OnCreate(b);

            var args = Arguments;
            this.server = args.GetString(KEY_FTP_SERVER);
            this.port = args.GetInt(KEY_FTP_PORT);
            this.username = args.GetString(KEY_FTP_USERNAME) ?? "anonymous";
            this.password = args.GetString(KEY_FTP_PASSWORD) ?? "anonymous";
            this.rootDir = args.GetString(KEY_FTP_ROOTDIR) ?? "/";

            ftp = new FtpClient(server, username, password) {UsePassive = true};
        }


        public override bool IsDir(FtpFile path)
        {
            return path.IsDirectory;
        }

        public override string GetName(FtpFile path)
        {
            return path.Name;
        }

        public override Uri ToUri(FtpFile path)
        {
            var user = "";
            if (!string.IsNullOrEmpty(username))
            {
                user = username;
                if (!string.IsNullOrEmpty(password))
                {
                    user += ":" + password;
                }
                user += "@";
            }
            return Uri.Parse("ftp://" + user + server + ":" + port + path.Path);
        }

        public override FtpFile GetParent(FtpFile from)
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

        public override string GetFullPath(FtpFile path)
        {
            return path.Path;
        }

        public override FtpFile GetPath(string path)
        {
            return new FtpFile(path);
        }

        public override FtpFile Root
        {
            get { return new FtpDir(rootDir); }
        }

        public override AbstractAsyncTaskLoader<FtpFile> Loader
        {
            get
            {
                return new AsyncFtpPickerTaskLoader(Activity,
                    (file) => file.IsDirectory || (Mode == FilePickerMode.File || Mode == FilePickerMode.FileAndDir), Root, CurrentPath, Mode, loggedIn, server, port, ftp, username, password);
            }
        }

        protected override void OnNewFodler(string name)
        {
            Task.Run(async () =>
            {
                var folder = await OnNewFolderAsync(name);
                if (folder != null)
                {
                    CurrentPath = folder;
                    Refresh();
                }
                else
                {
                    Toast.MakeText(Context, Resource.String.nnf_create_folder_error, ToastLength.Short).Show();
                }
            });
        }

        public async Task<FtpFile> OnNewFolderAsync(string name)
        {
            return await Task.Run(() =>
            {
                //var folder = new FtpDir(CurrentPath, name);
                //try
                //{
                //    if (ftp.MakeDirectory(folder.Path))
                //    {
                //        // Success, return result
                //        return folder;
                //    }
                //}
                //catch (IOException e)
                //{
                //    Log.e(TAG, "IO Exception: " + folder.getPath());
                //}
                return (FtpFile)null;
            });
        }
    }

}
