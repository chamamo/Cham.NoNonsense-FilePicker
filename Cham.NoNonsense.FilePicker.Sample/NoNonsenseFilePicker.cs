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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cham.NoNonsense.FilePicker.Sample.DropBox;
using Cham.NoNonsense.FilePicker.Sample.Ftp;
using Dropbox.CoreApi.Android;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker.Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class NoNonsenseFilePicker : Activity
    {

        private static int _codeSd = 0;
        private static int _codeDb = 1;
        private static int _codeFtp = 2;
        private TextView _textView;
        private DropboxApi mDBApi = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_no_nonsense_file_picker);

            var checkAllowCreateDir = (CheckBox) FindViewById(Resource.Id.checkAllowCreateDir);
            var checkAllowMultiple = (CheckBox) FindViewById(Resource.Id.checkAllowMultiple);
            var checkLightTheme = (CheckBox) FindViewById(Resource.Id.checkLightTheme);

            _textView = (TextView) FindViewById(Resource.Id.text);

            FindViewById(Resource.Id.button_sd).Click += (s, e) =>
            {
                Intent i;

                if (checkLightTheme.Checked)
                {
                    i = new Intent(this, typeof (FilePickerActivity2));
                }
                else
                {
                    i = new Intent(this,
                        typeof (FilePickerActivity));
                }
                i.SetAction(Intent.ActionGetContent);

                i.PutExtra(Const.ExtraAllowMultiple, checkAllowMultiple.Checked);
                i.PutExtra(Const.ExtraAllowCreateDir, checkAllowCreateDir.Checked);

                i.PutExtra(Const.ExtraMode, (int) FilePickerMode);


                StartActivityForResult(i, _codeSd);
            };


            FindViewById(Resource.Id.button_image).Click += (s, e) =>
            {
                var i = checkLightTheme.Checked
                    ? new Intent(this, typeof (MultimediaPickerActivity2))
                    : new Intent(this, typeof (MultimediaPickerActivity));
                i.SetAction(Intent.ActionGetContent);

                i.PutExtra(Const.ExtraAllowMultiple, checkAllowMultiple.Checked);
                i.PutExtra(Const.ExtraAllowCreateDir, checkAllowCreateDir.Checked);
                i.PutExtra(Const.ExtraMode, (int) FilePickerMode);
                StartActivityForResult(i, _codeSd);
            };

            FindViewById(Resource.Id.button_ftp).Click += (s, e) =>
            {
                Intent i;

                if (checkLightTheme.Checked)
                {
                    i = new Intent(this,
                        typeof (FtpPickerActivity2));
                }
                else
                {
                    i = new Intent(this,
                        typeof (FtpPickerActivity));
                }
                i.SetAction(Intent.ActionGetContent);

                i.PutExtra(Const.ExtraAllowMultiple, checkAllowMultiple.Checked);
                i.PutExtra(Const.ExtraAllowCreateDir, checkAllowCreateDir.Checked);

                // What mode is selected (makes no sense to restrict to folders here)
                var mode = FilePickerMode;
                i.PutExtra(Const.ExtraMode, (int) mode);
                StartActivityForResult(i, _codeFtp);
            };

            FindViewById(Resource.Id.button_dropbox).Click += (s, e) =>
            {
                if (mDBApi == null)
                {
                    mDBApi = DropboxSyncHelper.GetDBApi(this);
                }

                // If not authorized, then ask user for login/permission
                if (!mDBApi.Session.IsLinked)
                {
                    ((AndroidAuthSession) mDBApi.Session).StartOAuth2Authentication(this);
                }
                else
                {
                    // User is authorized, open file picker
                    var i = checkLightTheme.Checked
                        ? new Intent(this, typeof (DropboxFilePickerActivity2))
                        : new Intent(this, typeof (DropboxFilePickerActivity));
                    i.PutExtra(Const.ExtraAllowMultiple, checkAllowMultiple.Checked);
                    i.PutExtra(Const.ExtraAllowCreateDir, checkAllowCreateDir.Checked);

                    i.PutExtra(Const.ExtraMode, (int) FilePickerMode);
                    StartActivityForResult(i, _codeDb);
                }
            };
        }

        private FilePickerMode FilePickerMode
        {
            get
            {
                FilePickerMode mode;
                var radioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup);
                switch (radioGroup.CheckedRadioButtonId)
                {
                    case Resource.Id.radioDir:
                        mode = FilePickerMode.Dir;
                        break;
                    case Resource.Id.radioFilesAndDirs:
                        mode =
                                FilePickerMode.FileAndDir;
                        break;
                    case Resource.Id.radioFile:
                    default:
                        mode = FilePickerMode.File;
                        break;
                }
                return mode;
            }
        }

        /**
     * This is entirely for Dropbox's benefit
     */

        protected override void OnResume()
        {
            base.OnResume();

            if (mDBApi != null && ((AndroidAuthSession) mDBApi.Session).AuthenticationSuccessful())
            {
                try
                {
                    // Required to complete auth, sets the access token on the session
                    ((AndroidAuthSession)mDBApi.Session).FinishAuthentication();

                    var accessToken = ((AndroidAuthSession) mDBApi.Session).OAuth2AccessToken;
                    DropboxSyncHelper.SaveToken(this, accessToken);
                }
                catch (IllegalStateException e)
                {
                    Log.Info("DbAuthLog", "Error authenticating", e);
                }
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            // Inflate the menu; this adds items to the action bar if it is present.
            MenuInflater.Inflate(Resource.Menu.no_nonsense_file_picker, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // Handle action bar item clicks here. The action bar will
            // automatically handle clicks on the Home/Up button, so long
            // as you specify a parent activity in AndroidManifest.xml.
            int id = item.ItemId;
            return id == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if ((_codeSd == requestCode || _codeDb == requestCode || _codeFtp == requestCode) &&
                resultCode == Result.Ok)
            {
                if (data.GetBooleanExtra(Const.ExtraAllowMultiple, false))
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                    {
                        var clip = data.ClipData;
                        var sb = new StringBuilder();

                        if (clip != null)
                        {
                            for (int i = 0; i < clip.ItemCount; i++)
                            {
                                sb.Append(clip.GetItemAt(i).Uri.ToString());
                                sb.Append("\n");
                            }
                        }

                        _textView.Text = sb.ToString();
                    }
                    else
                    {
                        var paths = data.GetStringArrayListExtra(Const.ExtraPaths);
                        StringBuilder sb = new StringBuilder();

                        if (paths != null)
                        {
                            foreach (var path in paths)
                            {
                                sb.Append(path);
                                sb.Append("\n");
                            }
                        }
                        _textView.Text = sb.ToString();
                    }
                }
                else
                {
                    _textView.Text = data.Data.ToString();
                }
            }
        }

    }

}
