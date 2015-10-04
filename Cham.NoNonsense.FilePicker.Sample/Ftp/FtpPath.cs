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

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{
    public class FtpPath
    {
        public readonly string Path;
        public readonly FtpFile File;

        public FtpPath(string path, FtpFile file)
        {
            Path = path;
            this.File = file;
        }

        public FtpPath(FtpPath currentPath, FtpFile file)
        {
            File = file;
            if (currentPath.Path.EndsWith("/"))
            {
                Path = currentPath + file.Name;
            }
            else
            {
                Path = currentPath.Path + "/" + file.Name;
            }
        }

        public bool IsDirectory
        {
            get { return File.IsDirectory; }
        }

        public string Name
        {
            get { return File.Name; }
        }

        public string AppendToDir(string name)
        {
            if (Path.EndsWith("/"))
            {
                return Path + name;
            }
            else
            {
                return Path + "/" + name;
            }
        }
    }
}