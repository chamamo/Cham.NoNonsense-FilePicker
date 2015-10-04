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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Cham.NoNonsense.FilePicker;
using Java.Lang;
using Uri = Android.Net.Uri;
using Fragment = Android.Support.V4.App.Fragment;
using Loader = Android.Support.V4.Content.Loader;
using LoaderManager = Android.Support.V4.App.LoaderManager;
using Object = Java.Lang.Object;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Cham.NoNonsense.FilePicker
{
    public abstract class AbstractFilePickerFragment<T> : Fragment, LoaderManager.ILoaderCallbacks, ILogicHandler<T>
    {
        private const string KeyCurrentPath = "KEY_CURRENT PATH";

        protected readonly HashSet<T> CheckedItems;
        protected readonly HashSet<CheckableViewHolder<T>> CheckedVisibleViewHolders;
        protected FilePickerMode Mode = FilePickerMode.File;
        protected T CurrentPath = default(T);
        protected bool AllowCreateDir = false;
        protected bool AllowMultiple = false;
        protected IOnFilePickedListener Listener;
        protected FileItemAdapter<T> Adapter = null;
        protected TextView CurrentDirView;
        protected IList<T> Files = null;
        protected Toast Toast = null;

        protected AbstractFilePickerFragment()
        {
            CheckedItems = new HashSet<T>();
            CheckedVisibleViewHolders = new HashSet<CheckableViewHolder<T>>();

            // Retain this fragment across configuration changes, to allow
            // asynctasks and such to be used with ease.
            RetainInstance = true;
        }

        public T FirstCheckedItem
        {
            get { return CheckedItems.FirstOrDefault(); }
        }

        protected virtual bool HasPermission
        {
            // Nothing to request by default
            get { return true; }
        }

        public abstract T Root { get; }

        public abstract AbstractAsyncTaskLoader<T> Loader { get; }

        protected FileItemAdapter<T> GetAdapter()
        {
            return Adapter;
        }

        public void SetArgs(string startPath, FilePickerMode mode, bool allowMultiple, bool allowDirCreate)
        {
            // There might have been arguments set elsewhere, if so do not overwrite them.
            var b = Arguments ?? new Bundle();

            if (startPath != null)
            {
                b.PutString(Const.KeyStartPath, startPath);
            }
            b.PutBoolean(Const.KeyAllowDirCreate, allowDirCreate);
            b.PutBoolean(Const.KeyAllowMultiple, allowMultiple);
            b.PutInt(Const.KeyMode, (int) mode);
            Arguments = b;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.nnf_fragment_filepicker, container, false);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.nnf_picker_toolbar);
            if (toolbar != null)
            {
                SetupToolbar(toolbar);
            }

            var recyclerView = view.FindViewById<RecyclerView>(Resource.Id.list);
            // improve performance if you know that changes in content
            // do not change the size of the RecyclerView
            recyclerView.HasFixedSize = true;
            // use a linear layout manager
            LinearLayoutManager mLayoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(mLayoutManager);
            // Set adapter
            Adapter = new FileItemAdapter<T>(this);
            recyclerView.SetAdapter(Adapter);

            var buttonCancel = view.FindViewById(Resource.Id.nnf_button_cancel);
            buttonCancel.Click += (s, e) =>
            {
                OnClickCancel();
            };

            var buttonOk = view.FindViewById(Resource.Id.nnf_button_ok);
            buttonOk.Click += (s, e) =>
            {
                OnClickOk();
            };

            CurrentDirView = (TextView) view.FindViewById(Resource.Id.nnf_current_dir);
            // Restore state
            if (CurrentPath != null && CurrentDirView != null)
            {
                CurrentDirView.Text = GetFullPath(CurrentPath);
            }

            return view;
        }

        public void OnClickCancel()
        {
            if (Listener != null)
            {
                Listener.OnCancelled();
            }
        }

        public void OnClickOk()
        {
            if (Listener == null)
            {
                return;
            }

            // Some invalid cases first
            if ((AllowMultiple || Mode == FilePickerMode.File) && CheckedItems.Count == 0)
            {
                if (Toast == null)
                {
                    Toast = Toast.MakeText(Activity, Resource.String.nnf_select_something_first,
                        ToastLength.Short);
                }
                Toast.Show();
                return;
            }

            if (AllowMultiple)
            {
                Listener.OnFilesPicked(ToUri(CheckedItems));
            }
            else if (Mode == FilePickerMode.File)
            {
                Listener.OnFilePicked(ToUri(FirstCheckedItem));
            }
            else if (Mode == FilePickerMode.Dir)
            {
                Listener.OnFilePicked(ToUri(CurrentPath));
            }
            else
            {
                // single FILE OR DIR
                Listener.OnFilePicked(CheckedItems.Count == 0 ? ToUri(CurrentPath) : ToUri(FirstCheckedItem));
            }
        }

        protected void SetupToolbar(Toolbar toolbar)
        {
            ((AppCompatActivity) Activity).SetSupportActionBar(toolbar);
        }

        protected List<Android.Net.Uri> ToUri(IEnumerable<T> files)
        {
            return files.Select(ToUri).ToList();
        }

        public bool IsCheckable(T data)
        {
            bool checkable;
            if (IsDir(data))
            {
                checkable = ((Mode == FilePickerMode.Dir && AllowMultiple) ||
                             (Mode == FilePickerMode.FileAndDir && AllowMultiple));
            }
            else
            {
                // File
                checkable = (Mode != FilePickerMode.Dir);
            }
            return checkable;
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            try
            {
                Listener = (IOnFilePickedListener) context;
            }
            catch (ClassCastException)
            {
                throw new ClassCastException(context + " must implement IOnFilePickedListener");
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            // Only if we have no state
            if (CurrentPath == null)
            {
                if (savedInstanceState != null)
                {
                    Mode = (FilePickerMode) savedInstanceState.GetInt(Const.KeyMode, (int) Mode);
                    AllowCreateDir = savedInstanceState
                        .GetBoolean(Const.KeyAllowDirCreate, AllowCreateDir);
                    AllowMultiple = savedInstanceState.GetBoolean(Const.KeyAllowMultiple, AllowMultiple);
                    CurrentPath = GetPath(savedInstanceState.GetString(KeyCurrentPath));
                }
                else if (Arguments != null)
                {
                    Mode = (FilePickerMode) Arguments.GetInt(Const.KeyMode, (int) Mode);
                    AllowCreateDir = Arguments.GetBoolean(Const.KeyAllowDirCreate, AllowCreateDir);
                    AllowMultiple = Arguments.GetBoolean(Const.KeyAllowMultiple, AllowMultiple);
                    if (Arguments.ContainsKey(Const.KeyStartPath))
                    {
                        CurrentPath = GetPath(Arguments.GetString(Const.KeyStartPath));
                    }
                }

                // If still null
                if (CurrentPath == null)
                {
                    CurrentPath = Root;
                }
            }

            Refresh();
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            inflater.Inflate(Resource.Menu.picker_actions, menu);

            var item = menu.FindItem(Resource.Id.nnf_action_createdir);
            item.SetVisible(AllowCreateDir);
        }

        public override bool OnOptionsItemSelected(IMenuItem menuItem)
        {
            if (Resource.Id.nnf_action_createdir == menuItem.ItemId)
            {
                Activity activity = Activity;
                var compatActivity = activity as AppCompatActivity;
                if (compatActivity != null)
                {
                    NewFolderFragment.ShowDialog(compatActivity.SupportFragmentManager, OnNewFodler);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void OnNewFodler(string name)
        {

        }

        public override void OnSaveInstanceState(Bundle b)
        {
            base.OnSaveInstanceState(b);
            b.PutString(KeyCurrentPath, CurrentPath.ToString());
            b.PutBoolean(Const.KeyAllowMultiple, AllowMultiple);
            b.PutBoolean(Const.KeyAllowDirCreate, AllowCreateDir);
            b.PutInt(Const.KeyMode, (int) Mode);
        }

        public override void OnDetach()
        {
            base.OnDetach();
            Listener = null;
        }

        protected void Refresh()
        {
            if (HasPermission)
            {
                LoaderManager.RestartLoader(0, null, this);
            }
            else
            {
                HandlePermission();
            }
        }

        protected virtual void HandlePermission()
        {
            // Nothing to do by default
        }

        public Android.Support.V4.Content.Loader OnCreateLoader(int id, Bundle args)
        {
            return Loader;
        }

        public void OnLoadFinished(Loader loader, Object data)
        {
            CheckedItems.Clear();
            CheckedVisibleViewHolders.Clear();
            var list = (JavaObjectWrapper<T[]>) data;
            Files = (IList<T>) list.Obj.ToList();
            Adapter.List = Files;
            if (CurrentDirView != null)
            {
                CurrentDirView.Text = GetFullPath(CurrentPath);
            }
        }

        public void OnLoaderReset(Android.Support.V4.Content.Loader loader)
        {
            Adapter.List = null;
            Files = null;
        }

        public virtual int GetItemViewType(int position, T data)
        {
            if (IsCheckable(data))
            {
                return (int) LogicHandlerViewType.Checkable;
            }
            else
            {
                return (int) LogicHandlerViewType.Dir;
            }
        }

        public abstract bool IsDir(T path);

        public abstract string GetName(T path);

        public abstract Android.Net.Uri ToUri(T path);

        public abstract T GetParent(T @from);

        public abstract string GetFullPath(T path);

        public abstract T GetPath(string path);

        public void OnBindHeaderViewHolder(HeaderViewHolder viewHolder)
        {
            viewHolder.Text.Text = "..";
        }

        public virtual RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View v;
            switch ((LogicHandlerViewType) viewType)
            {
                case LogicHandlerViewType.Header:
                    v = LayoutInflater.From(Activity)
                        .Inflate(Resource.Layout.nnf_filepicker_listitem_dir, parent, false);
                    var headerViewHolder = new HeaderViewHolder(v);
                    headerViewHolder.Click += (s, e) =>
                    {
                        OnClickHeader(headerViewHolder);
                    };
                    return headerViewHolder;
                case LogicHandlerViewType.Checkable:
                    v = LayoutInflater.From(Activity).Inflate(Resource.Layout.nnf_filepicker_listitem_checkable,
                        parent, false);
                    var checkableView = new CheckableViewHolder<T>(v);
                    checkableView.Checkbox.Click += (s, e) =>
                    {
                        OnClickCheckBox(checkableView);
                    };
                    checkableView.Click += (s, e) =>
                    {
                        OnClickCheckBox(checkableView);
                    };
                    checkableView.LongClick += (s, e) =>
                    {
                        OnLongClickCheckable(checkableView);
                    };
                    return checkableView;
                case LogicHandlerViewType.Dir:
                default:
                    v = LayoutInflater.From(Activity).Inflate(Resource.Layout.nnf_filepicker_listitem_dir, parent, false);
                    var dirViewHolder =  new DirViewHolder<T>(v);
                    dirViewHolder.Click += (s, e) =>
                    {
                        OnClickDir(dirViewHolder);
                    };
                    dirViewHolder.LongClick += (s, e) =>
                    {
                        OnLongClickDir(dirViewHolder);
                    };
                    return dirViewHolder;
            }
        }

        public virtual void OnBindViewHolder(DirViewHolder<T> vh, int position, T data)
        {
            vh.File = data;
            vh.Icon.Visibility = IsDir(data) ? ViewStates.Visible : ViewStates.Gone;
            vh.Text.Text = GetName(data);

            if (IsCheckable(data))
            {
                if (CheckedItems.Contains(data))
                {
                    CheckedVisibleViewHolders.Add((CheckableViewHolder<T>) vh);
                    ((CheckableViewHolder<T>) vh).Checkbox.Checked = true;
                }
                else
                {
                    //noinspection SuspiciousMethodCalls
                    var cvh = (CheckableViewHolder<T>) vh;
                    CheckedVisibleViewHolders.Remove(cvh);
                    (cvh).Checkbox.Checked = false;
                }
            }
        }

        public void ClearSelections()
        {
            foreach (var vh in CheckedVisibleViewHolders)
            {
                vh.Checkbox.Checked = false;
            }
            CheckedVisibleViewHolders.Clear();
            CheckedItems.Clear();
        }

        public void OnClickHeader(HeaderViewHolder viewHolder)
        {
            GoUp();
        }

        public void GoUp()
        {
            GoToDir(GetParent(CurrentPath));
        }

        public void OnClickDir(DirViewHolder<T> viewHolder)
        {
            if (IsDir(viewHolder.File))
            {
                GoToDir(viewHolder.File);
            }
        }

        public void GoToDir(T file)
        {
            CurrentPath = file;
            CheckedItems.Clear();
            CheckedVisibleViewHolders.Clear();
            Refresh();
        }

        protected virtual bool OnLongClickDir(DirViewHolder<T> viewHolder)
        {
            return false;
        }

        protected virtual void OnClickCheckable( CheckableViewHolder<T> viewHolder)
        {
            if (IsDir(viewHolder.File))
            {
                GoToDir(viewHolder.File);
            }
            else
            {
                OnLongClickCheckable(viewHolder);
            }
        }

        public bool OnLongClickCheckable(CheckableViewHolder<T> viewHolder)
        {
            OnClickCheckBox(viewHolder);
            return true;
        }

        public void OnClickCheckBox(CheckableViewHolder<T> viewHolder)
        {
            if (CheckedItems.Contains(viewHolder.File))
            {
                viewHolder.Checkbox.Checked = false;
                CheckedItems.Remove(viewHolder.File);
                CheckedVisibleViewHolders.Remove(viewHolder);
            }
            else
            {
                if (!AllowMultiple)
                {
                    ClearSelections();
                }
                viewHolder.Checkbox.Checked = true;
                CheckedItems.Add(viewHolder.File);
                CheckedVisibleViewHolders.Add(viewHolder);
            }
        }
    }
}