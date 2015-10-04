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

namespace Cham.NoNonsense.FilePicker.Sample.Ftp
{
    public class FtpFile
    {

        public static readonly char separatorChar = '/';
        public static readonly String separator = "/";

        public FtpFile(FtpFile dir, string name)
            : this(dir == null ? null : dir.Path, name)
        {

        }

        public FtpFile(string path)
        {
            Path = FixSlashes(path);
        }

        public FtpFile(string dirPath, String name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(dirPath))
            {
                Path = FixSlashes(name);
            }
            else if (string.IsNullOrEmpty(name))
            {
                Path = FixSlashes(dirPath);
            }
            else
            {
                Path = FixSlashes(Join(dirPath, name));
            }
        }

        public string Name
        {
            get
            {
                var separatorIndex = Path.LastIndexOf(separator, StringComparison.Ordinal);
                return (separatorIndex < 0) ? Path : Path.Substring(separatorIndex + 1, Path.Length - (separatorIndex + 1));
            }
        }

        public string Parent
        {
            get
            {
                int length = Path.Length;
                var firstInPath = 0;
                int index = Path.LastIndexOf(separatorChar);
                if (index == -1 || Path[length - 1] == separatorChar)
                {
                    return null;
                }
                if (Path.IndexOf(separatorChar) == index
                    && Path[firstInPath] == separatorChar)
                {
                    return Path.Substring(0, index + 1);
                }
                return Path.Substring(0, index);
            }
        }

        public FtpFile ParentFile
        {
            get
            {
                var tempParent = Parent;
                if (tempParent == null)
                {
                    return null;
                }
                return new FtpFile(tempParent);
            }
        }

        /**
     * Returns the path of this file.
     */
        public string Path { get; private set; }

        public virtual bool IsDirectory
        {
            get { return false; }
        }

        public virtual bool IsFile
        {
            get { return true; }
        }

        public static String FixSlashes(String origPath)
        {
            // Remove duplicate adjacent slashes.
            var lastWasSlash = false;
            var newPath = origPath.ToCharArray();
            var length = newPath.Length;
            var newLength = 0;
            for (var i = 0; i < length; ++i)
            {
                var ch = newPath[i];
                if (ch == '/')
                {
                    if (lastWasSlash) continue;
                    newPath[newLength++] = separatorChar;
                    lastWasSlash = true;
                }
                else
                {
                    newPath[newLength++] = ch;
                    lastWasSlash = false;
                }
            }
            // Remove any trailing slash (unless this is the root of the file system).
            if (lastWasSlash && newLength > 1)
            {
                newLength--;
            }
            // Reuse the original string if possible.
            return (newLength != length) ? new String(newPath, 0, newLength) : origPath;
        }

        // Joins two path components, adding a separator only if necessary.
        public static string Join(string prefix, string suffix)
        {
            int prefixLength = prefix.Length;
            var haveSlash = (prefixLength > 0 && prefix[prefixLength - 1] == separatorChar);
            if (!haveSlash)
            {
                haveSlash = (suffix.Length > 0 && suffix[0] == separatorChar);
            }
            return haveSlash ? (prefix + suffix) : (prefix + separatorChar + suffix);
        }
    }
}