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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using DialogFragment = Android.Support.V4.App.DialogFragment;

namespace Cham.NoNonsense.FilePicker
{
    public abstract class NewItemFragment : DialogFragment, IDialogInterfaceOnShowListener
    {
        public EventHandler<string> OnNewFolder;

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            builder.SetView(Resource.Layout.nnf_dialog_folder_name)
                .SetTitle(Resource.String.nnf_new_folder)
                .SetNegativeButton(Android.Resource.String.Cancel, (s,e)=>{})
                .SetPositiveButton(Android.Resource.String.Ok, (s, e) => { });

            var dialog = builder.Create();
            dialog.SetOnShowListener(this);
            return dialog;
        }

        public void OnShow(IDialogInterface dialog1)
        {
            var dialog = (AlertDialog)dialog1;
            var editText = dialog.FindViewById<EditText>(Resource.Id.edit_text);

            var cancel = dialog.GetButton((int)DialogButtonType.Negative);
            cancel.Click += (s, e) =>
            {
                Dialog.Cancel();
            };

            var ok = dialog.GetButton((int)DialogButtonType.Positive);
            // Start disabled
            ok.Enabled = false;
            ok.Click += (s, e) =>
            {
                var itemName = editText.Text;
                if (ValidateName(itemName))
                {
                    if (OnNewFolder != null) OnNewFolder(this, itemName);
                    dialog.Dismiss();
                }
            };
            editText.AfterTextChanged += (s, e) =>
            {
                ok.Enabled = ValidateName(s.ToString());
            };
        }

        protected abstract bool ValidateName(string itemName);
    }
}
