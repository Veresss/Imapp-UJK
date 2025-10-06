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
        string userName = Preferences.Get("UserName", "Vend�g");
    }

    private void UpdateGreeting()
    {
        string userName = Preferences.Get("UserName", "Vend�g");
        string greeting;
        var hour = DateTime.Now.Hour;
        if (hour < 10) { greeting = "J� reggelt"; }
        else if (hour < 14) { greeting = "Sz�p napot"; }
        else if (hour < 18) { greeting = "J�, hogy itt vagy"; }
        else { greeting = "J� est�t"; }
        GreetingLabel.Text = $"{greeting}, {userName}!";
    }

    private void UpdateReadings()
    {
        var today = DateTime.Now.Date; // Mai d�tum
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
                await DisplayAlert("Hiba", "Hi�nyz� c�loldal.", "OK");
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
            Title = "Napi Imaid�m",
            Description = "Itt �ll�thatod be, hogy mennyit szeretn�l im�zni egy alkalommal. Kaphatsz jelz�st is, ha letelik ez az id�.",
            Icon = "prayerpageicon.png",
            TargetPageType = typeof(PrayerPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "Tervez�",
            Description = "Szem�lyre szabhatod a hetedet, hogy melyik napon �s mikor szeretn�l id�t sz�nni Istenre.",
            Icon = "plannerpageicon.png",
            TargetPageType = typeof(PlannerPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "K�z�ss�g",
            Description = "Ossz meg m�sokkal igehelyeket, melyeket aktu�lisnak �rzel, �s meg�rintettek valamilyen m�don.",
            Icon = "communitypageicon.png",
            TargetPageType = typeof(CommunityPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "S.O.S.",
            Description = "Nem tudod �ppen hogyan im�zz? Ne agg�dj, itt gyorsan elk�sz�thet�nk neked egy imatervet egy id�keretben.",
            Icon = "helperpageicon.png",
            TargetPageType = typeof(HelperPage)
        });
        SlideItems.Add(new SlideItem
        {
            Title = "Be�ll�t�sok",
            Description = "K�rhetsz push �rtes�t�st a telefonodra, m�dos�thatod a nevet, �s app st�lust v�laszthatsz.",
            Icon = "notificationpageicon.png",
            TargetPageType = typeof(SettingPage)
        });
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}