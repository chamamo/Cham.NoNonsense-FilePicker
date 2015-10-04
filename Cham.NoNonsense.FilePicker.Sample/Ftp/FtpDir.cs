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

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{
    public class FtpDir : FtpFile
    {
        public FtpDir(string path)
            : base(path)
        {
        }

        public FtpDir(FtpFile dir, string name)
            : base(dir, name)
        {

        }

        public FtpDir(string parentPath, string filename)
            : base(parentPath, filename)
        {

        }

        public override bool IsDirectory
        {
            get { return true; }
        }

        public override bool IsFile
        {
            get { return false; }
        }
    }
}