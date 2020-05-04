using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LocationPermissions
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            MessagingCenter.Subscribe<object>("EVENTLOCATION", "GRANTED",sender =>
            {
                MessagingCenter.Unsubscribe<object>("EVENTLOCATION", "GRANTED");
                //load again according to permission response.
                DisplayAlert("Loading","Yes permission Granted","ok");
            });

            InitializeComponent();
        }

        

        async Task checkPermission()
        {
            //prompt for location permission
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS)
            {
                try
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                    if (status == PermissionStatus.Granted)
                    {
                        //permission granted
                        //do your stuff related to geoloactions
                        await DisplayAlert("Location Permission", "Location Permission Granted. You can use any GeoLocation functions.", "OK");
                        //DependencyService.Get<ILocationPermission>().RedirectToSettings();
                    }

                    else if (status == PermissionStatus.Denied)
                    {
                        //permission Denied in android devices
                            // TODO : ios device permission is denied we cant call the permission prompt again redirect to settings page
                            //var requstStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>(); //cant call this iOS dont allow to prompt again
                            await DisplayAlert("Location Permission", "Location Permission Denied. Please go to settings and allow the location permissions", "OK");
                            DependencyService.Get<ILocationPermission>().RedirectToSettings();
                    }

                    else if (status == PermissionStatus.Unknown)
                    {
                        //ask me next tym is set tat comes to unknown
                        //permission Unkown aka permission denied in ios devices
                        try
                        {
                            var requstStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                            if(requstStatus == PermissionStatus.Granted)
                            {
                                //allow once - when u check next need to ask again for permission
                                //while using the app - when you check next tym its already granter
                                await DisplayAlert("Location Permission", "Location Permission Granted. You can use any Geolocations functions.", "OK");
                            }
                            else if(requstStatus == PermissionStatus.Denied)
                            {
                                //dont allow
                                await DisplayAlert("Location Permission", "Location Permission Denied.If you use any GeoLocations functions you will get exceptions.", "OK");
                            }
                            else if(requstStatus == PermissionStatus.Disabled)
                            {
                                //ios don't get into this
                            }
                            else if(requstStatus == PermissionStatus.Restricted)
                            {
                                //ios don't get into this
                            }
                            else if(requstStatus == PermissionStatus.Unknown)
                            {
                                //ios don't get into this
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }

                    else if (status == PermissionStatus.Restricted)
                    {
                        //permission restricted
                        //ios don't get into this
                    }

                    else if (status == PermissionStatus.Disabled)
                    {
                        //locaiton service disabled prompt user to switch on locations
                        await DisplayAlert("Location Service", "Location Service Disabled.Please go to settings and enable the location service", "OK");
                        DependencyService.Get<ILocationPermission>().RedirectToSettings();
                    }
                }
                catch(Exception e)
                {
                    throw e;
                }
            }

            if(Device.RuntimePlatform == Device.Android)
            {
                try
                {

                    bool result = await DependencyService.Get<ILocationPermission>().CheckEventLocationPermission();
                    if (result)
                    {
                        //permissoin granted
                        //await DisplayAlert("RESULTS","SUCCESS","OK");
                    }
                    else
                    {
                        //permission denied or permission prompt send to settings.
                        //await DisplayAlert("RESULTS", "FAIL", "OK");
                    }

                }

                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Page2());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            checkPermission();
        }
    }
}
