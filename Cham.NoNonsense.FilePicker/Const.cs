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

namespace Cham.NoNonsense.FilePicker
{
    public static class Const
    {
        public const string ExtraStartPath = "nononsense.intent" + ".START_PATH";
        public static readonly string ExtraMode = "nononsense.intent.MODE";
        public static readonly string ExtraAllowCreateDir = "nononsense.intent" + ".ALLOW_CREATE_DIR";
        // For compatibility
        public static readonly string ExtraAllowMultiple = "android.intent.extra" + ".ALLOW_MULTIPLE";
        public static readonly string ExtraPaths = "nononsense.intent.PATHS";

        // Where to display on open.
        public static readonly string KeyStartPath = "KEY_START_PATH";
        // See MODE_XXX constants above for possible values
        public static readonly string KeyMode = "KEY_MODE";
        // If it should be possible to create directories.
        public static readonly string KeyAllowDirCreate = "KEY_ALLOW_DIR_CREATE";
        // Allow multiple items to be selected.
        public static readonly string KeyAllowMultiple = "KEY_ALLOW_MULTIPLE";
        // Used for saving state.
        
    }
}