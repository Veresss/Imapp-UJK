using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmptyMauiApp.Services;

namespace EmptyMauiApp.Views;

public partial class StylePage : BasePage
{
    private Border[] styleBorders;

    public StylePage()
    {
        InitializeComponent();
        styleBorders = new Border[]
        {
            DefaultBorder,     // 0: Alapértelmezett

            BrightBorder,      // 8: Világos
            DarkBorder         // 9: Sötét
        };
    }

    private async Task AnimateWidth(Border border, double targetWidth, uint duration = 250)
    {
        var animation = new Animation(v => border.WidthRequest = v, border.WidthRequest, targetWidth);
        animation.Commit(border, "WidthAnimation", 16, duration, Easing.SinInOut);
        await Task.Delay((int)duration);
    }

    private async Task SelectStyleAsync(int index, bool animate = true)
    {
        var tasks = new List<Task>();
        for (int i = 0; i < styleBorders.Length; i++)
        {
            double targetWidth = (i == index) ? 360 : 320; // 360 px a kiválasztottra, 320 px az alap
            if (animate)
            {
                tasks.Add(AnimateWidth(styleBorders[i], targetWidth));
            }
            else
            {
                styleBorders[i].WidthRequest = targetWidth;
            }
        }

        if (animate)
        {
            await Task.WhenAll(tasks);
        }

        ThemeManager.SetPreset(index);
    }

    ///*
    private async void OnStyleOneTapped(object sender, EventArgs e)
    {
        await SelectStyleAsync(0);
    }
    //*/

    private async void OnStyleBrightTapped(object sender, EventArgs e)
    {
        await SelectStyleAsync(1);
    }

    private async void OnStyleDarkTapped(object sender, EventArgs e)
    {
        await SelectStyleAsync(2);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
        this.ForceLayout();

        // Kezdeti állapot beállítása animáció nélkül
        int currentPreset = ThemeManager.GetCurrentPreset();
        SelectStyleAsync(currentPreset, animate: false);
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}