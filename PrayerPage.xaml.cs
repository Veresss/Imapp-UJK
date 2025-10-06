using EmptyMauiApp.Services;
using EmptyMauiApp.Views.CustomControls;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;

namespace EmptyMauiApp.Views;

public partial class PrayerPage : BasePage
{
    private int _selectedMinutes = 20;
    private int _totalSeconds => _selectedMinutes * 60;

    public static readonly BindableProperty IsTimerRunningProperty =
        BindableProperty.Create(nameof(IsTimerRunning), typeof(bool), typeof(PrayerPage), false);

    public bool IsTimerRunning
    {
        get => (bool)GetValue(IsTimerRunningProperty);
        set => SetValue(IsTimerRunningProperty, value);
    }

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(PrayerPage), 0.0);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public PrayerPage()
    {
        InitializeComponent();
        LoadSavedTime();
        ProgressView.Progress = 0;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Feliratkozás az eseményekre
        TimerService.Instance.Tick += OnTimerTick;
        TimerService.Instance.Completed += OnTimerCompleted;

        // Ha már fut az időzítő (pl. visszatérés az oldalra), helyreállítjuk az állapotot
        if (TimerService.Instance.IsRunning)
        {
            IsTimerRunning = true;
            SetRadioButtonsEnabled(false);
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            UpdateTimerUI(TimerService.Instance.RemainingSeconds);
        }

        RefreshVisuals();
        this.ForceLayout();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // Leiratkozás az eseményekről (de az időzítő fut tovább a singletonban)
        TimerService.Instance.Tick -= OnTimerTick;
        TimerService.Instance.Completed -= OnTimerCompleted;
    }

    private async void UpdateTimerUI(int remainingSeconds)
    {
        double targetProgress = (double)remainingSeconds / _totalSeconds;
        Progress = targetProgress; // Bindolható tulajdonság frissítése
        int minutesLeft = (int)Math.Ceiling(remainingSeconds / 60.0);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimerLabel.Text = $"{minutesLeft} perc";
            ProgressView.Progress = targetProgress;
        });

        await ProgressView.ProgressTo(targetProgress, 1000, Easing.Linear);
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        if (IsTimerRunning) return;

        TimerService.Instance.Start(_totalSeconds);
        IsTimerRunning = true;
        SetRadioButtonsEnabled(false);
        StartButton.IsEnabled = false;
        StopButton.IsEnabled = true;
        UpdateTimerUI(_totalSeconds);
    }

    private void OnStopClicked(object sender, EventArgs e)
    {
        TimerService.Instance.Stop();
        ResetUI();
    }

    private void OnTimerTick(int remainingSeconds)
    {
        UpdateTimerUI(remainingSeconds);
    }

    private void OnTimerCompleted()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Időzítő", "Lejárt az idő!", "OK");
            ResetUI();
        });
    }

    private void ResetUI()
    {
        IsTimerRunning = false;
        Progress = 0;
        SetRadioButtonsEnabled(true);
        StartButton.IsEnabled = true;
        StopButton.IsEnabled = false;
        TimerLabel.Text = $"{_selectedMinutes} perc";
        ProgressView.Progress = 0;
    }

    private void LoadSavedTime()
    {
        try
        {
            if (Preferences.Default.ContainsKey("SelectedPrayerTime"))
            {
                var savedValue = Preferences.Default.Get("SelectedPrayerTime", "20");

                if (int.TryParse(savedValue, out int savedMinutes))
                {
                    if (savedMinutes >= 20 && savedMinutes <= 60 && savedMinutes % 5 == 0)
                    {
                        _selectedMinutes = savedMinutes;
                    }
                }
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerLabel.Text = $"{_selectedMinutes} perc";
                SelectRadioButton(_selectedMinutes);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Hiba a beállítások betöltésekor: {ex}");
            _selectedMinutes = 20;
        }
    }

    private void SelectRadioButton(int minutes)
    {
        try
        {
            UncheckAllRadioButtons();

            switch (minutes)
            {
                case 20: RadioButton20.IsChecked = true; break;
                case 25: RadioButton25.IsChecked = true; break;
                case 30: RadioButton30.IsChecked = true; break;
                case 35: RadioButton35.IsChecked = true; break;
                case 40: RadioButton40.IsChecked = true; break;
                case 45: RadioButton45.IsChecked = true; break;
                case 50: RadioButton50.IsChecked = true; break;
                case 55: RadioButton55.IsChecked = true; break;
                case 60: RadioButton60.IsChecked = true; break;
                default: RadioButton20.IsChecked = true; break;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Hiba a RadioButton kiválasztásakor: {ex}");
        }
    }

    private void UncheckAllRadioButtons()
    {
        RadioButton20.IsChecked = false;
        RadioButton25.IsChecked = false;
        RadioButton30.IsChecked = false;
        RadioButton35.IsChecked = false;
        RadioButton40.IsChecked = false;
        RadioButton45.IsChecked = false;
        RadioButton50.IsChecked = false;
        RadioButton55.IsChecked = false;
        RadioButton60.IsChecked = false;
    }

    private void SetRadioButtonsEnabled(bool enabled)
    {
        RadioButton20.IsEnabled = enabled;
        RadioButton25.IsEnabled = enabled;
        RadioButton30.IsEnabled = enabled;
        RadioButton35.IsEnabled = enabled;
        RadioButton40.IsEnabled = enabled;
        RadioButton45.IsEnabled = enabled;
        RadioButton50.IsEnabled = enabled;
        RadioButton55.IsEnabled = enabled;
        RadioButton60.IsEnabled = enabled;
    }

    private void OnTimeSelectionChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton)
        {
            try
            {
                _selectedMinutes = Convert.ToInt32(radioButton.Value);
                Preferences.Default.Set("SelectedPrayerTime", _selectedMinutes.ToString());
                TimerLabel.Text = $"{_selectedMinutes} perc";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Hiba az idő mentésekor: {ex}");
            }
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}