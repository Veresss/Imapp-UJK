using EmptyMauiApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmptyMauiApp.Views;

public partial class SettingPage : BasePage
{
    private bool _notificationsEnabled;
    private const string NotificationsEnabledKey = "NotificationsEnabled";
    private const string UsernameKey = "Username";

    public SettingPage()
    {
        InitializeComponent();
        LoadSettings();
        NotificationsSwitch.IsToggled = _notificationsEnabled;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await NotificationService.UpdateNotificationsAsync(); // update on loading
        RefreshVisuals();
        this.ForceLayout();
    }

    private void LoadSettings()
    {
        _notificationsEnabled = Preferences.Get(NotificationsEnabledKey, false); //false?
    }

    private async void OnNotificationsToggled(object sender, ToggledEventArgs e)
    {
        _notificationsEnabled = e.Value;
        Preferences.Set(NotificationsEnabledKey, _notificationsEnabled);
        await NotificationService.UpdateNotificationsAsync(); // update
    }

    private async void OnUsernameEditTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UsernameEditPage());
    }

    private async void OnStyleEditTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StylePage());
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}