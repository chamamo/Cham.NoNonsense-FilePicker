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

using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Net;
using Android.Support.V7.App;

namespace Cham.NoNonsense.FilePicker
{
    public abstract class AbstractFilePickerActivity<T> : AppCompatActivity, IOnFilePickedListener
    {
        private const string Tag = "filepicker_fragment";
        protected string StartPath = null;
        protected FilePickerMode Mode = FilePickerMode.File;
        protected bool AllowCreateDir = false;
        protected bool AllowMultiple = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.nnf_activity_filepicker);

            var intent = Intent;
            if (intent != null)
            {
                StartPath = intent.GetStringExtra(Const.ExtraStartPath);
                Mode = (FilePickerMode) intent.GetIntExtra(Const.ExtraMode, (int) Mode);
                AllowCreateDir = intent.GetBooleanExtra(Const.ExtraAllowCreateDir, AllowCreateDir);
                AllowMultiple = intent.GetBooleanExtra(Const.ExtraAllowMultiple, AllowMultiple);
            }
            var fm = SupportFragmentManager;
            var fragment = (AbstractFilePickerFragment<T>) fm.FindFragmentByTag(Tag) ?? GetFragment(StartPath, Mode, AllowMultiple, AllowCreateDir);
            if (fragment != null)
            {
                fm.BeginTransaction().Replace(Resource.Id.fragment, fragment, Tag).Commit();
            }

            // Default to cancelled
            SetResult(Result.Canceled);
        }

        protected abstract AbstractFilePickerFragment<T> GetFragment(string startPath, FilePickerMode mode, bool allowMultiple, bool allowCreateDir);

        public void OnFilePicked(Uri file)
        {
            var i = new Intent();
            i.SetData(file);
            SetResult(Result.Ok, i);
            Finish();
        }

        public void OnFilesPicked(List<Android.Net.Uri> files)
        {
            var i = new Intent();
            i.PutExtra(Const.ExtraAllowMultiple, true);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                ClipData clip = null;
                foreach (var file in files)
                {
                    if (clip == null)
                    {
                        clip = new ClipData("Paths", new string[] {},
                            new ClipData.Item(file));
                    }
                    else
                    {
                        clip.AddItem(new ClipData.Item(file));
                    }
                }
                i.ClipData = clip;
            }
            else
            {
                var paths = files.Select(file => file.ToString()).ToList();
                i.PutStringArrayListExtra(Const.ExtraPaths, paths);
            }

            SetResult(Result.Ok, i);
            Finish();
        }

        public void OnCancelled()
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }
}
