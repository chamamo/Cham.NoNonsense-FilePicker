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

using System.Linq;
using System.Threading;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;

namespace Cham.NoNonsense.FilePicker.Sample
{

public class MultimediaPickerFragment : FilePickerFragment {

    // Make sure these do not collide with LogicHandler.VIEWTYPE codes.
    // They are 1-2, so 11 leaves a lot of free space in between.
    private static readonly int VIEWTYPE_IMAGE_CHECKABLE = 11;
    private static readonly int VIEWTYPE_IMAGE = 12;

    private static readonly string[] MULTIMEDIA_EXTENSIONS =
            new string[]{".png", ".jpg", ".gif", ".mp4"};

    /**
     * An extremely simple method for identifying multimedia. This
     * could be improved, but it's good enough for this example.
     *
     * @param file which could be an image or a video
     * @return true if the file can be previewed, false otherwise
     */

    protected bool IsMultimedia(File file)
    {
        //noinspection SimplifiableIfStatement
        if (IsDir(file))
        {
            return false;
        }

        var path = file.Path.ToLower();
        return MULTIMEDIA_EXTENSIONS.Any(ext => path.EndsWith(ext));
    }

    public override int GetItemViewType(int position, File file) {
        if (IsMultimedia(file)) {
            if (IsCheckable(file)) {
                return VIEWTYPE_IMAGE_CHECKABLE;
            } else {
                return VIEWTYPE_IMAGE;
            }
        } else {
            return base.GetItemViewType(position, file);
        }
    }

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        if (viewType == VIEWTYPE_IMAGE_CHECKABLE)

            return new CheckableViewHolder<File>(LayoutInflater.From(Activity)
                .Inflate(Resource.Layout.listitem_image_checkable, parent, false));
        if (viewType == VIEWTYPE_IMAGE)
            return new DirViewHolder<File>(LayoutInflater.From(Activity)
                .Inflate(Resource.Layout.listitem_image, parent, false));

        return base.OnCreateViewHolder(parent, viewType);

    }

    public override void OnBindViewHolder(DirViewHolder<File> vh, int position, File file)
    {
        // Let the super method do its thing with checkboxes and text
        base.OnBindViewHolder(vh, position, file);

        // Here we load the preview image if it is an image file
        var viewType = GetItemViewType(position, file);
        if (viewType == VIEWTYPE_IMAGE_CHECKABLE || viewType == VIEWTYPE_IMAGE)
        {
            // Need to set it to visible because the base code will set it to invisible by default
            vh.Icon.Visibility = ViewStates.Visible;
            // Just load the image
            ThreadPool.QueueUserWorkItem(o =>
            {
                var bitmap = DecodeSampledBitmapFromResource(file.AbsolutePath, 50, 50);
                Activity.RunOnUiThread(() => vh.Icon.SetImageBitmap(bitmap));
            });
        }
    }

    private static Bitmap DecodeSampledBitmapFromResource(string strPath, int reqWidth, int reqHeight)
    {

        // First decode with inJustDecodeBounds=true to check dimensions

        var options = new BitmapFactory.Options();
        options.InJustDecodeBounds = true;
        BitmapFactory.DecodeFile(strPath, options);
        // Calculate inSampleSize
        options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);
        // Decode bitmap with inSampleSize set
        options.InJustDecodeBounds = false;
        return BitmapFactory.DecodeFile(strPath, options);

    }

    public static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
    {
        // Raw height and width of image
        int height = options.OutHeight;
        int width = options.OutWidth;
        int inSampleSize = 1;
        if (height > reqHeight || width > reqWidth)
        {

            int halfHeight = height/2;
            int halfWidth = width/2;

            // Calculate the largest inSampleSize value that is a power of 2 and
            // keeps both
            // height and width larger than the requested height and width.
            while ((halfHeight/inSampleSize) > reqHeight
                   && (halfWidth/inSampleSize) > reqWidth)
            {
                inSampleSize *= 2;
            }
        }
        return inSampleSize;
    }
}

}
