using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android;
using Android.Util;
using Xamarin.Forms;

namespace LocationPermissions.Droid
{
    [Activity(Label = "LocationPermissions", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Context FormsContext { get; set; }

        public static Activity ThisActivity { get; set; }

        private readonly string[] locationPermissions =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        private const int LOCATIONPERMISSIONREQUEST = 101;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            ThisActivity = this;
            FormsContext = this;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 200)
            {
                // Received permission result for Location permission.
                Log.Info("Location", "Received response for Location permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.
                    Log.Info("Location", "Location permission has now been granted.");
                    MessagingCenter.Send<object>("EVENTLOCATION", "GRANTED");
                }
                else
                {
                    Log.Info("Location", "Location permission was NOT granted.");
                    //string[] requiredPermissions = new string[] { Manifest.Permission.AccessFineLocation };
                    //Android.Views.View view = FindViewById(Android.Resource.Id.Content);
                }
            }

            //handle location permissions results here from dependency service
            if (requestCode == LOCATIONPERMISSIONREQUEST)
            {
                // Received permission result for Location permission.
                Log.Info("Location", "Received response for Location permission request.");

                // Check if the only required permission has been granted
                if ((grantResults.Length == 1) && (grantResults[0] == Permission.Granted))
                {
                    // Location permission has been granted, okay to retrieve the location of the device.

                }
                else
                {
                    //permission denied
                }
            }

        }

        protected override void OnStart()
        {
            base.OnStart();
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) != Permission.Granted)
                {
                    RequestPermissions(locationPermissions, LOCATIONPERMISSIONREQUEST);
                }
            }
        }
    }
}