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
using Android.Net;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;

namespace Cham.NoNonsense.FilePicker
{
    public interface ILogicHandler<T>
    {

        

        /**
         * Return true if the path is a directory and not a file.
         *
         * @param path
         */
        bool IsDir(T path);

        /**
         * @param path
         * @return filename of path
         */
        string GetName(T path);

        /**
         * Convert the path to a URI for the return intent
         *
         * @param path
         * @return a Uri
         */
        Uri ToUri(T path);

        /**
         * Return the path to the parent directory. Should return the root if
         * from is root.
         *
         * @param from
         */
        T GetParent(T from);

        /**
         * @param path
         * @return the full path to the file
         */
        string GetFullPath(T path);

        /**
         * Convert the path to the type used.
         *
         * @param path
         */
        T GetPath(string path);

        /**
         * Get the root path (lowest allowed).
         */
        T Root { get; }

        /**
         * Get a loader that lists the files in the current path,
         * and monitors changes.
         */
        AbstractAsyncTaskLoader<T> Loader { get; }

        /**
         * Bind the header ".." which goes to parent folder.
         *
         * @param viewHolder
         */
        void OnBindHeaderViewHolder(HeaderViewHolder viewHolder);

        /**
         * Header is subtracted from the position
         *
         * @param parent
         * @param viewType
         * @return a view holder for a file or directory
         */
        RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType);

        /**
         * @param viewHolder to bind data from either a file or directory
         * @param position   0 - n, where the header has been subtracted
         * @param data
         */
        void OnBindViewHolder(DirViewHolder<T> viewHolder, int position, T data);

        /**
         * @param position 0 - n, where the header has been subtracted
         * @param data
         * @return an integer greater than 0
         */
        int GetItemViewType(int position, T data);
    }

}
