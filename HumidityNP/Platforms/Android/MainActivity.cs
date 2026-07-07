using Android.App;
using Android.Content.PM;
using Android.OS;

namespace HumidityNP;

[Activity(Theme = "@style/MyTransparentSplash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        RequestPermissions();
    }

    private void RequestPermissions()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            RequestPermissions(new[] {
                Android.Manifest.Permission.BluetoothScan,
                Android.Manifest.Permission.BluetoothConnect,
                Android.Manifest.Permission.AccessFineLocation
            }, 101);
        }
        else
        {
            RequestPermissions(new[] {
                Android.Manifest.Permission.AccessFineLocation,
                Android.Manifest.Permission.AccessCoarseLocation
            }, 101);
        }
    }
}