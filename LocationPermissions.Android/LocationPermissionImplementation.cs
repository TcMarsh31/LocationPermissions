using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using LocationPermissions.Droid;
using Xamarin.Forms;
/*
* GPS Location reference
*https://stackoverflow.com/questions/10311834/how-to-check-if-location-services-are-enabled 
*/

[assembly: Dependency(typeof(LocationPermissionImplementation))]
namespace LocationPermissions.Droid
{
    public class LocationPermissionImplementation : ILocationPermission
    {
        
        public LocationPermissionImplementation()
        {
        }

        public async Task<bool> CheckEventLocationPermission()
        {
            try
            {
                Activity activity = MainActivity.ThisActivity;
                LocationManager locationManager = (LocationManager)activity.GetSystemService(Context.LocationService);
                bool isGPSEnabled = locationManager.IsProviderEnabled(LocationManager.GpsProvider);

                //If Version greater than Marshallow need to give run time permission
                if (isGPSEnabled)
                {
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                    {
                        if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
                        {
                            // We have been given permission.
                            return true;
                        }
                        else { 
                            // If necessary display rationale & request.
                            if (ActivityCompat.ShouldShowRequestPermissionRationale(activity, Manifest.Permission.AccessFineLocation))
                            {
                                // Provide an additional rationale to the user if the permission was not granted
                                // and the user would benefit from additional context for the use of the permission.
                                // For example if the user has previously denied the permission.
                                //Log.Info("Location", "Displaying Location permission rationale to provide additional context.");

                                var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
                                ActivityCompat.RequestPermissions(activity, requiredPermissions, 200);
                                return false;
                            }
                            else
                            {
                                //Show alertbox to prompt user to enable location services in app level
                                AlertDialog.Builder alert = new AlertDialog.Builder(activity);
                                alert.SetTitle("Location Permission");
                                alert.SetCancelable(false);
                                alert.SetMessage("App needs location permission to access your current location");

                                alert.SetPositiveButton("OK", (senderAlert, args) =>
                                {
                                    MainActivity.FormsContext.StartActivity(new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings, Android.Net.Uri.Parse("package:com.xamarinlife.locationpermissions")));
                                });

                                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                                {
                                    //MessagingCenter.Send<object>("EVENTLOCATION", "REJECTED");
                                });

                                Dialog dialog = alert.Create();
                                dialog.Show();
                                return false;
                            }
                        }
                    }
                    //Less than Marshmallow version so we can request directly
                    else
                    {
                        ActivityCompat.RequestPermissions(activity, new String[] { Manifest.Permission.AccessFineLocation }, 200);
                        return false;
                    }
                }
                else
                {
                    //show popup to redirect to settings page to enable GPS
                    AlertDialog.Builder alert = new AlertDialog.Builder(activity);
                    alert.SetTitle("GPS ");
                    alert.SetCancelable(false);
                    alert.SetMessage("GPS is disabled. Please enable GPS to access full feature.");

                    alert.SetPositiveButton("OK", (senderAlert, args) =>
                    {
                        MainActivity.FormsContext.StartActivity(new Intent(Android.Provider.Settings.ActionSettings));
                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                    {
                        //MessagingCenter.Send<object>("EVENTLOCATION", "REJECTED");
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task<bool> CheckEventLocationPermissionIOS()
        {
            throw new NotImplementedException();
        }

        public bool GetLocationStatus()
        {
            Activity activity = MainActivity.ThisActivity;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.AccessFineLocation) == (int)Permission.Granted)
                {
                    // We have permission, go ahead and use the location.
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void RedirectToSettings()
        {
            throw new NotImplementedException();
        }
    }
}
