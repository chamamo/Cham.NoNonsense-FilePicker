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
using Android.Support.V7.Widget;
using Android.Views;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker
{

    public class FileItemAdapter<T> : RecyclerView.Adapter
    {

        private readonly ILogicHandler<T> _logic;
        

        public FileItemAdapter(ILogicHandler<T> logic)
        {
            this._logic = logic;
        }

        private IList<T> _list;
        public IList<T> List
        {
            get { return _list; }
            set
            {
                _list = value;
                NotifyDataSetChanged();
            }
        }

        public override int ItemCount
        {
            get
            {
                if (_list == null)
                {
                    return 0;
                }

                // header + count
                return 1 + _list.Count;
            }
        }


        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return _logic.OnCreateViewHolder(parent, viewType);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int headerPosition)
        {
            if (headerPosition == 0)
            {
                _logic.OnBindHeaderViewHolder((HeaderViewHolder)viewHolder);
            }
            else
            {
                int pos = headerPosition - 1;
                _logic.OnBindViewHolder((DirViewHolder<T>)viewHolder, pos, _list[pos]);
            }
        }

        public override int GetItemViewType(int headerPosition)
        {
            if (0 == headerPosition)
            {
                return (int) LogicHandlerViewType.Header;
            }
            else
            {
                int pos = headerPosition - 1;
                return _logic.GetItemViewType(pos, _list[pos]);
            }
        }
    }

}


