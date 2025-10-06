using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace EmptyMauiApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState) // '?' helyzete
    {
        base.OnCreate(savedInstanceState);

        Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
        Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);

        Window.SetDecorFitsSystemWindows(false);

        Window.InsetsController?.Hide(WindowInsets.Type.StatusBars() | WindowInsets.Type.NavigationBars());
    }
}
