using System;
using System.Threading.Tasks;

namespace LocationPermissions
{
    public interface ILocationPermission
    {
        Task<bool> CheckEventLocationPermission();

        Task<bool> CheckEventLocationPermissionIOS();

        bool GetLocationStatus();

        void RedirectToSettings();
    }
}
