using System;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Foundation;
using LocalAuthentication;
using LocationPermissions.iOS;
using UIKit;
using Xamarin.Essentials;
using static Xamarin.Essentials.AppleSignInAuthenticator;

[assembly: Xamarin.Forms.Dependency(typeof(LocationPermissionImplementation))]
namespace LocationPermissions.iOS
{
    public class LocationPermissionImplementation : ILocationPermission
    {
        CLLocationManager locMgr = new CLLocationManager();

        public LocationPermissionImplementation()
        {
        }

        public bool CheckEventLocationPermission()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckEventLocationPermissionIOS()
        {
            try
            {
                var context = new LAContext();


                if (CLLocationManager.LocationServicesEnabled)
                {
                    if (CLLocationManager.Status == CLAuthorizationStatus.Denied || CLLocationManager.Status == CLAuthorizationStatus.Restricted)
                    {
                        var okCancelAlertController = UIAlertController.Create("Location Services are disabled", "CISI needs location permission to access your current location.", UIAlertControllerStyle.Alert);
                        okCancelAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) => RedirectToSettingsPage()));
                        okCancelAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

                        UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(okCancelAlertController, true, null);
                        return false;
                    }
                    else if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
                    {
                        if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                        {
                            locMgr.RequestWhenInUseAuthorization();
                            //locMgr.RequestAlwaysAuthorization();
                            locMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                            {
                                LocationsUpdated(locMgr, e.Locations);
                            };
                        }
                        else
                        {
                            locMgr.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) =>
                            {
                                //LocationsUpdated(locMgr, e.Locations);
                            };
                        }
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetLocationStatus()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                if (CLLocationManager.Status == CLAuthorizationStatus.Denied || CLLocationManager.Status == CLAuthorizationStatus.Restricted)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #region Location Manager Methods

        [Export("locationManager:didUpdateLocations:")]
        public void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            CLLocation lastLocation = locations == null ? null : locations.LastOrDefault();
            if (lastLocation == null)
                return;
        }

        [Export("locationManager:didFailWithError:")]
        public void Failed(CLLocationManager manager, NSError error)
        {
            Console.WriteLine("System: Location Manager Error: {0}", error);
        }

        [Export("locationManager:didChangeAuthorizationStatus:")]
        public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status)
        {
            manager.RequestLocation();
        }

        #endregion

        private async void RedirectToSettingsPage()
        {

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Denied)
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(UIKit.UIApplication.OpenSettingsUrlString));
                }
                else if (status == PermissionStatus.Disabled)
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("prefs:root=LOCATION_SERVICES"));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            //await OpenUrlAsync("ms-settings:privacy-location");
        }

        public async void RedirectToSettings()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status == PermissionStatus.Denied)
                {
                    bool r = UIApplication.SharedApplication.OpenUrl(new NSUrl(UIKit.UIApplication.OpenSettingsUrlString));
                }
                else if (status == PermissionStatus.Disabled)
                {
                    bool res = UIApplication.SharedApplication.OpenUrl(new NSUrl("App-Prefs:root=Privacy&path=Location-Services"));
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
        
}
