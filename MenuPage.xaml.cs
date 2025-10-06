using EmptyMauiApp.Models;
using EmptyMauiApp.Services;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace EmptyMauiApp.Views;

public partial class MenuPage : BasePage, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<SlideItem> SlideItems { get; } = new();
    public ICommand SlideTappedCommand { get; }

    private string _currentGospel;
    public string CurrentGospel
    {
        get => _currentGospel;
        set
        {
            _currentGospel = value;
            OnPropertyChanged(nameof(CurrentGospel));
        }
    }

    private string _currentReading;
    public string CurrentReading
    {
        get => _currentReading;
        set
        {
            _currentReading = value;
            OnPropertyChanged(nameof(CurrentReading));
        }
    }

    public MenuPage()
    {
        InitializeComponent();
        UpdateGreeting();
        SlideTappedCommand = new Command<SlideItem>(OnSlideTapped);
        BindingContext = this;
        LoadSlideItems();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateReadings();
        UpdateUsername();
        RefreshVisuals();
        this.ForceLayout();
    }

    private void UpdateUsername()
    {
        string userName = Preferences.Get("UserName", "Vendég");
    }

    private void UpdateGreeting()
    {
        string userName = Preferences.Get("UserName", "Vendég");
        string greeting;
        var hour = DateTime.Now.Hour;
        if (hour < 10) { greeting = "Jó reggelt"; }
        else if (hour < 14) { greeting = "Szép napot"; }
        else if (hour < 18) { greeting = "Jó, hogy itt vagy"; }
        else { greeting = "Jó estét"; }
        GreetingLabel.Text = $"{greeting}, {userName}!";
    }

    private void UpdateReadings()
    {
        var today = DateTime.Now.Date; // Mai dátum
        var reading = ReadingService.GetReadingForDate(today);
        CurrentGospel = reading.Gospel;
        CurrentReading = reading.FirstReading;
    }

    private async void OnSlideTapped(SlideItem slide)
    {
        try
        {
            if (slide?.TargetPageType == null)
            {
                await DisplayAlert("Hiba", "Hiányzó céloldal.", "OK");
                return;
            }

            var page = (Page?)Activator.CreateInstance(slide.TargetPageType);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Hiba", ex.Message, "OK");
            Debug.WriteLine($"Hiba: {ex}");
        }
    }

    private void LoadSlideItems()
    {
        SlideItems.Add(new SlideItem
        {
            Title = "Napi Imaidõm",
            Description = "Itt állíthatod be, hogy mennyit szeretnél imázni egy alkalommal. Kaphatsz jelzést is, ha letelik ez az idõ.",
            Icon = "prayerpageicon.png",
            TargetPageType = typeof(PrayerPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "Tervezõ",
            Description = "Személyre szabhatod a hetedet, hogy melyik napon és mikor szeretnél idõt szánni Istenre.",
            Icon = "plannerpageicon.png",
            TargetPageType = typeof(PlannerPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "Közösség",
            Description = "Ossz meg másokkal igehelyeket, melyeket aktuálisnak érzel, és megérintettek valamilyen módon.",
            Icon = "communitypageicon.png",
            TargetPageType = typeof(CommunityPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "S.O.S.",
            Description = "Nem tudod éppen hogyan imázz? Ne aggódj, itt gyorsan elkészíthetünk neked egy imatervet egy idõkeretben.",
            Icon = "helperpageicon.png",
            TargetPageType = typeof(HelperPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "Beállítások",
            Description = "Kérhetsz push értesítést a telefonodra, módosíthatod a nevet, és app stílust választhatsz.",
            Icon = "notificationpageicon.png",
            TargetPageType = typeof(SettingPage)
        });
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}