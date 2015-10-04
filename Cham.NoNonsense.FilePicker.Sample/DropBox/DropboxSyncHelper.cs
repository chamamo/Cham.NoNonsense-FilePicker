using Android.Content;
using Android.Preferences;
using Dropbox.CoreApi.Android;
using Dropbox.CoreApi.Android.Session;

namespace Cham.NoNonsense.FilePicker.Sample.DropBox
{
    public class DropboxSyncHelper {
    // Change these two lines to your app's stuff
        static public string AppKey = "csujk6zdyx4smvd";
        static public string AppSecret = "6vxtdatdcc7dtl9";

    public static string PrefDropboxToken = "dropboxtoken";

        public static DropboxApi GetDBApi(Context context)
        {
            var appKeys = new AppKeyPair(AppKey, AppSecret);
            AndroidAuthSession session;

            if (PreferenceManager.GetDefaultSharedPreferences(context).Contains(PrefDropboxToken))
            {
                session = new AndroidAuthSession(appKeys,
                    PreferenceManager.GetDefaultSharedPreferences(context).GetString(PrefDropboxToken, ""));
            }
            else
            {
                session = new AndroidAuthSession(appKeys);
            }
            return new DropboxApi(session);
        }

        public static void SaveToken(Context context, string token)
        {
            PreferenceManager.GetDefaultSharedPreferences(context).Edit().PutString(PrefDropboxToken, token).Apply();
        }
    }
}