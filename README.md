# Cham.NoNonsense-FilePicker

This is work of [spacecowboy](https://github.com/spacecowboy)'s [NoNonsense-FilePicker](https://github.com/spacecowboy/NoNonsense-FilePicker), which has been ported to C#.

<p>
<img src="https://raw.githubusercontent.com/spacecowboy/NoNonsense-FilePicker/master/screenshots/Nexus6-picker-dark.png"
width="25%"
</img>

<img src="https://raw.githubusercontent.com/spacecowboy/NoNonsense-FilePicker/master/screenshots/Nexus10-picker-dark.png"
width="50%"
</img>
</p>

<p>
<img src="https://raw.githubusercontent.com/spacecowboy/NoNonsense-FilePicker/master/screenshots/Nexus6-picker-light.png"
width="25%"
</img>

<img src="https://raw.githubusercontent.com/spacecowboy/NoNonsense-FilePicker/master/screenshots/Nexus10-picker-light.png"
width="50%"
</img>
</p>

-   Extendable for sources other than SD-card (Dropbox, FTP, Drive, etc)
-   Can select multiple items
-   Select directories or files, or both
-   Create new directories in the picker
-   Material theme with AppCompat

## Yet another file picker library?

I needed a file picker that had two primary properties:

1.  Easy to extend: I needed a file picker that would work for normal
    files on the SD-card, and also for using the Dropbox API.
2.  Able to create a directory in the picker.

This project has both of those qualities. As a bonus, it also scales
nicely to work on any phone or tablet. The core is placed in abstract
classes, so it is fairly easy to extend the picker to create
your own.

The library includes an implementation that allows the user to pick
files from the SD-card. But the picker could easily be extended to get
its file listings from another source, such as Dropbox, FTP, SSH and
so on. The sample app includes implementations which browses your
Dropbox and a Linux mirror FTP-server.

By inheriting from an Activity, the picker is able to be rendered as
full screen on small screens and as a dialog on large screens. It does
this through the theme system, so it is very important for the
activity to use a correctly configured theme.


## How to use the included SD-card picker:

### Include permission in your manifest

```xml
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
```

### Include the file picker activity

The intent filter is optional depending on your use case. Note that
the theme set in the manifest is important.

```xml
    <activity
       android:name="com.nononsenseapps.filepicker.FilePickerActivity"
       android:label="@string/app_name"
       android:theme="@style/FilePickerTheme">
       <intent-filter>
          <action android:name="android.intent.action.GET_CONTENT" />
          <category android:name="android.intent.category.DEFAULT" />
       </intent-filter>
    </activity>
```

### Configure the theme

You must **set the theme** on the activity, but you can configure it to
match your existing application theme. You can also name it whatever
you like..

```xml
    <!-- You can also inherit from NNF_BaseTheme.Light -->
    <style name="FilePickerTheme" parent="NNF_BaseTheme">
        <!-- Set these to match your theme -->
        <item name="colorPrimary">@color/primary</item>
        <item name="colorPrimaryDark">@color/primary_dark</item>
        <item name="colorAccent">@color/accent</item>

        <!-- Need to set this also to style create folder dialog -->
        <item name="alertDialogTheme">@style/FilePickerAlertDialogTheme</item>

        <!-- If you want to set a specific toolbar theme, do it here -->
        <!-- <item name="nnf_toolbarTheme">@style/ThemeOverlay.AppCompat.Dark.ActionBar</item> -->
    </style>

    <style name="FilePickerAlertDialogTheme" parent="Theme.AppCompat.Dialog.Alert">
        <item name="colorPrimary">@color/primary</item>
        <item name="colorPrimaryDark">@color/primary_dark</item>
        <item name="colorAccent">@color/accent</item>
    </style>
```

### Starting the picker in your app

```csharp
    // This always works
    Intent i = new Intent(context, typeof(FilePickerActivity));
    // This works if you defined the intent filter
    // Intent i = new Intent(Intent.ActionGetContent);

    // Set these depending on your use case. These are the defaults.
    i.PutExtra(Const.ExtraAllowMultiple, false);
    i.PutExtra(Const.ExtraAllowCreateDir, false);
    i.PutExtra(Const.ExtraMode, FilePickerMode.File);

    // Configure initial directory by specifying a String.
    // You could specify a String like "/storage/emulated/0/", but that can
    // dangerous. Always use Android's API calls to get paths to the SD-card or
    // internal memory.
    i.PutExtra(Const.ExtraStartPath, Environment.ExternalStorageDirectory.Path);

    StartActivityForResult(i, FILE_CODE);
```

### Handling the result

If you have a minimum requirement of Jelly Bean (API 16) and above,
you can skip the second method.

```csharp
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent data) {
        if (requestCode == FILE_CODE && resultCode == Result.Ok) {
            if (data.GetBooleanExtra(Const.ExtraAllowMultiple, false)) 
            {
                // For JellyBean and above
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                {
                    ClipData clip = data.ClipData;

                    if (clip != null) {
                        for (int i = 0; i < clip.ItemCount; i++)
                        {
                            Uri uri = clip.GetItemAt(i).Uri;
                            // Do something with the URI
                        }
                    }
                // For Ice Cream Sandwich
                } else 
                {
                    var paths = data.GetStringArrayListExtra(Const.ExtraPaths);

                    if (paths != null) {
                        foreach (var path in paths) {
                            Uri uri = Uri.Parse(path);
                            // Do something with the URI
                        }
                    }
                }

            } else {
                Uri uri = data.Data;
                // Do something with the URI
            }
        }
    }
```


See the sample project for examples on dark and light themes, and
implementations using Dropbox and FTP.

## Changelog

See [release notes](https://github.com/spacecowboy/Cham.NoNonsense-FilePicker/blob/master/release-notes.md)
