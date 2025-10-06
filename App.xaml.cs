using EmptyMauiApp.Views;
using System.Globalization;
using EmptyMauiApp.Services;
using Microsoft.Maui.Storage;

namespace EmptyMauiApp;

public partial class App : Application
{
    
    public App()
	{
		InitializeComponent();
        ThemeManager.SetPreset(Preferences.Get("CurrentPreset", 0));
        ThemeManager.SetPreset(0);
    }
	
	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new NavigationPage(new StartPage())); //NavigationPage becsomagolas pluszba!!!
	}
}