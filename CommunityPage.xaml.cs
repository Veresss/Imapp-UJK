using EmptyMauiApp.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace EmptyMauiApp.Views;

public partial class CommunityPage : BasePage
{
    public CommunityPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();  // navigacios veremben visszalepes
    }
}
