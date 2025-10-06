using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;

namespace EmptyMauiApp.Views;

public partial class UsernameEditPage : BasePage
{
    private const string UsernameKey = "UserName";

    public UsernameEditPage()
    {
        InitializeComponent();
        LoadUsername();
    }

    private void LoadUsername()
    {
        string currentUsername = Preferences.Get(UsernameKey, string.Empty);
        NameEditEntry.Text = currentUsername;
        EditButton.IsEnabled = !string.IsNullOrWhiteSpace(currentUsername);
    }

    private void OnNameEditTextChanged(object sender, TextChangedEventArgs e)
    {
        EditButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        string userName = NameEditEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(userName))
        {
            Preferences.Set(UsernameKey, userName);
            //await DisplayAlert("Siker", "A felhasználónév sikeresen módosítva!", "OK");
            await Navigation.PopAsync();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}