
using System;
using System.Diagnostics;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace EmptyMauiApp.Views;

public partial class NamePage : BasePage
{
    public NamePage()
    {
        InitializeComponent();
    }

    private void OnNameTextChanged(object sender, TextChangedEventArgs e)
    {
        ContinueButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        string userName = NameEntry.Text.Trim();

        if (!string.IsNullOrEmpty(userName))
        {
            Preferences.Set("UserName", userName);
            await Navigation.PushAsync(new MenuPage());
        }
    }
}
