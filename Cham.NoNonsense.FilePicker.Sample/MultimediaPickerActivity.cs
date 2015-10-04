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
using Android.OS;
using Java.IO;

namespace Cham.NoNonsense.FilePicker.Sample
{
    [Activity]
    public class MultimediaPickerActivity : AbstractFilePickerActivity<File>
    {

        public MultimediaPickerActivity()
        {
        }


        protected override AbstractFilePickerFragment<File> GetFragment(string startPath, FilePickerMode mode,
            bool allowMultiple, bool allowCreateDir)
        {
            var fragment = new MultimediaPickerFragment();
            // startPath is allowed to be null. In that case, default folder should be SD-card and not "/"
            fragment.SetArgs(startPath ?? Environment.ExternalStorageDirectory.Path, mode, allowMultiple, allowCreateDir);
            return fragment;
        }
    }

}