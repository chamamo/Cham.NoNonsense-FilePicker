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
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Cham.NoNonsense.FilePicker
{
    public class DirViewHolder<T> : RecyclerView.ViewHolder
    {
        public readonly ImageView Icon;
        public readonly TextView Text;
        public T File;
        public EventHandler Click;
        public EventHandler LongClick;

        public DirViewHolder(View v)
            : base(v)
        {

            v.Click += (s, e) =>
            {
                if (Click != null)
                {
                    Click(this, EventArgs.Empty);
                }
            };
            v.LongClick += (s, e) =>
            {
                if (LongClick != null)
                {
                    LongClick(this, EventArgs.Empty);
                }
            };
            Icon = v.FindViewById<ImageView>(Resource.Id.item_icon);
            Text = (TextView)v.FindViewById(Resource.Id.text1);
        }
    }
}