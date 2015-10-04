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
using Android.Support.V4.App;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker
{

    public class NewFolderFragment : NewItemFragment
    {
        private const string TAG = "new_folder_fragment";

        public static void ShowDialog(FragmentManager fm, Action<string> onNewItem)
        {
            var d = new NewFolderFragment();
            d.OnNewFolder += (s, e) =>
            {
                onNewItem(e);
            };
            d.Show(fm, TAG);
        }

        protected override bool ValidateName(string itemName)
        {
            return !string.IsNullOrEmpty(itemName)
                   && !itemName.Contains("/")
                   && !itemName.Equals(".")
                   && !itemName.Equals("..");
        }
    }

}
