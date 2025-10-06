using EmptyMauiApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace EmptyMauiApp.Views;

public partial class PlannerPage : BasePage
{
    public ObservableCollection<DayModel> DaysOfWeek { get; } = new();
    private readonly Dictionary<DayModel, Border> _dayBorders = new();
    private DayModel _currentlyExpandedDay;

    private const string DaysPreferencesKey = "WeeklyPlannerSettings";
    private const string GlobalSwitchPreferencesKey = "GlobalSwitchSetting";
    private const string GlobalTimePreferencesKey = "GlobalTimeSetting";

    private bool _globalSwitchIsToggled;
    private TimeSpan _globalSelectedTime;

    public PlannerPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadSavedSettings();
        BuildManualLayout();
        ApplyGlobalSettingsToControls();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await SaveSettingsAsync();
    }

    private async Task SaveSettingsAsync()
    {
        var daysJson = JsonSerializer.Serialize(DaysOfWeek.ToList());
        Preferences.Set(DaysPreferencesKey, daysJson);

        Preferences.Set(GlobalSwitchPreferencesKey, _globalSwitchIsToggled);
        Preferences.Set(GlobalTimePreferencesKey, _globalSelectedTime.ToString());

        await NotificationService.UpdateNotificationsAsync(); //kilepeskor frissiti a valtozasokat
    }

    private void LoadDays()
    {
        DaysOfWeek.Clear();

        var days = new[]
        {
            ("Hétfő", "Monday"),
            ("Kedd", "Tuesday"),
            ("Szerda", "Wednesday"),
            ("Csütörtök", "Thursday"),
            ("Péntek", "Friday"),
            ("Szombat", "Saturday"),
            ("Vasárnap", "Sunday")
        };

        for (int i = 0; i < days.Length; i++)
        {
            var day = new DayModel
            {
                DayName = days[i].Item1,
                IsEnabled = false,
                SelectedTime = new TimeSpan(8, 0, 0),
                ZIndex = i + 1
            };
            DaysOfWeek.Add(day);
        }
    }

    private void LoadSavedSettings()
    {
        if (Preferences.ContainsKey(DaysPreferencesKey))
        {
            var json = Preferences.Get(DaysPreferencesKey, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                var savedDays = JsonSerializer.Deserialize<List<DayModel>>(json);
                if (savedDays != null && savedDays.Count == 7)
                {
                    DaysOfWeek.Clear();
                    foreach (var day in savedDays)
                    {
                        DaysOfWeek.Add(day);
                    }
                }
                else
                {
                    LoadDays();
                }
            }
            else
            {
                LoadDays();
            }
        }
        else
        {
            LoadDays();
        }

        _globalSwitchIsToggled = Preferences.Get(GlobalSwitchPreferencesKey, false);

        var savedTime = Preferences.Get(GlobalTimePreferencesKey, string.Empty);
        if (!string.IsNullOrEmpty(savedTime) && TimeSpan.TryParse(savedTime, out var time))
        {
            _globalSelectedTime = time;
        }
        else
        {
            _globalSelectedTime = new TimeSpan(8, 0, 0);
        }
    }

    private void ApplyGlobalSettingsToControls()
    {
        GlobalSwitch.IsToggled = _globalSwitchIsToggled;
        GlobalTimePicker.Time = _globalSelectedTime;
    }

    private void SaveSettings()
    {
        var daysJson = JsonSerializer.Serialize(DaysOfWeek.ToList());
        Preferences.Set(DaysPreferencesKey, daysJson);

        Preferences.Set(GlobalSwitchPreferencesKey, _globalSwitchIsToggled);
        Preferences.Set(GlobalTimePreferencesKey, _globalSelectedTime.ToString());
    }

    private void BuildManualLayout()
    {
        DaysContainer.Children.Clear();
        _dayBorders.Clear();

        foreach (var day in DaysOfWeek)
        {
            var border = CreateDayBorder(day);
            DaysContainer.Children.Add(border);
            _dayBorders[day] = border;
        }
    }

    //---BORDER ELEJE

    private Border CreateDayBorder(DayModel day)
    {
        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };

        gradientBrush.GradientStops.Add(new GradientStop
        {
            Color = (Color)Application.Current.Resources["PrimaryColor"],
            Offset = 0.0f
        });
        gradientBrush.GradientStops.Add(new GradientStop
        {
            Color = (Color)Application.Current.Resources["SecondaryTransparent"],
            Offset = 1.0f
        });

        var border = new Border
        {
            Padding = new Thickness(0),
            StrokeThickness = 2,
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
            Background = gradientBrush,
            Margin = new Thickness(20, 0, 20, -35),
            RotationX = -5,
            HeightRequest = 120,
            VerticalOptions = LayoutOptions.Start,
            ZIndex = day.ZIndex,
            BindingContext = day
        };
        border.SetDynamicResource(Border.StrokeProperty, "SecondaryTransparent");

        var layout = new VerticalStackLayout
        {
            Spacing = 10,
            Padding = new Thickness(10)
        };

        var dayLabel = new Label
        {
            Padding = new Thickness(0),
            Text = day.DayName,
            FontFamily = "AptosExtrabold",
            FontSize = 20,
            HorizontalOptions = LayoutOptions.Center
        };
        dayLabel.SetDynamicResource(Label.TextColorProperty, "SecondaryColor");

        var daySwitch = new Microsoft.Maui.Controls.Switch
        {
            Margin = new Thickness(10, 0, 10, 0),
            HorizontalOptions = LayoutOptions.Center,
            Scale = 1.5
        };
        daySwitch.SetDynamicResource(Switch.ThumbColorProperty, "PrimaryColor");
        daySwitch.SetBinding(Switch.IsToggledProperty, "IsEnabled", BindingMode.TwoWay);

        var timePicker = new Microsoft.Maui.Controls.TimePicker
        {
            Margin = new Thickness(20, 0, 20, 0),
            Format = "HH:mm",
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = 100,
            FontSize = 22,
            FontFamily = "AptosExtrabold",
            BackgroundColor = Colors.Transparent
        };
        timePicker.SetDynamicResource(TimePicker.TextColorProperty, "PrimaryColor");
        timePicker.SetBinding(TimePicker.TimeProperty, "SelectedTime", BindingMode.TwoWay);

        var expandedLayout = new StackLayout
        {
            IsVisible = false,
            Children = { daySwitch, timePicker }
        };

        layout.Children.Add(dayLabel);
        layout.Children.Add(expandedLayout);

        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCardTapped;
        border.GestureRecognizers.Add(tapGesture);

        border.Content = layout;
        return border;
    }

    //---BORDER VEGE

    private async void OnCardTapped(object sender, EventArgs e)
    {
        if (sender is Border tappedBorder && tappedBorder.BindingContext is DayModel tappedDay)
        {
            if (tappedDay.IsExpanded)
            {
                tappedDay.IsExpanded = false;
                tappedDay.ZIndex = DaysOfWeek.IndexOf(tappedDay) + 1;
                await CollapseBorder(tappedBorder);
                await SaveSettingsAsync();
                return;
            }

            if (_currentlyExpandedDay != null)
            {
                _currentlyExpandedDay.IsExpanded = false;
                _currentlyExpandedDay.ZIndex = DaysOfWeek.IndexOf(_currentlyExpandedDay) + 1;
                if (_dayBorders.TryGetValue(_currentlyExpandedDay, out var previousBorder))
                {
                    await CollapseBorder(previousBorder);
                }
            }

            tappedDay.IsExpanded = true;
            //tappedDay.ZIndex = 1000;
            var expandedLayout = FindExpandedLayout(tappedBorder);
            if (expandedLayout != null)
            {
                expandedLayout.IsVisible = true;
            }
            await ExpandBorder(tappedBorder);
            _currentlyExpandedDay = tappedDay;
            (tappedBorder.Parent as Microsoft.Maui.Controls.VisualElement)?.InvalidateMeasure();
            await SaveSettingsAsync();
        }
    }

    private async Task ExpandBorder(Border border)
    {
        border.AbortAnimation("ExpandAnimation");

        var expandedLayout = FindExpandedLayout(border);
        if (expandedLayout == null) return;

        var heightAnimation = new Animation(v => border.HeightRequest = v, 120, 220);
        var translateAnimation = new Animation(v => border.TranslationY = v, 0, -20);
        var opacityAnimation = new Animation(v => expandedLayout.Opacity = v, 0, 1);
        var rotationAnimation = new Animation(v => border.RotationX = v, border.RotationX, 0);

        var taskCompletionSource = new TaskCompletionSource<bool>();

        var compositeAnimation = new Animation();
        compositeAnimation.Add(0, 1, heightAnimation);
        compositeAnimation.Add(0, 1, translateAnimation);
        compositeAnimation.Add(0, 1, opacityAnimation);
        compositeAnimation.Add(0, 1, rotationAnimation);

        compositeAnimation.Commit(border, "ExpandAnimation", 16, 250, Easing.SpringOut,
            (v, c) => taskCompletionSource.TrySetResult(!c));

        await taskCompletionSource.Task;
    }

    private async Task CollapseBorder(Border border)
    {
        border.AbortAnimation("CollapseAnimation");

        var expandedLayout = FindExpandedLayout(border);
        if (expandedLayout == null) return;

        var heightAnimation = new Animation(v => border.HeightRequest = v, border.HeightRequest, 120);
        var translateAnimation = new Animation(v => border.TranslationY = v, border.TranslationY, 0);
        var opacityAnimation = new Animation(v => expandedLayout.Opacity = v, 1, 0);
        var rotationAnimation = new Animation(v => border.RotationX = v, border.RotationX, -5);

        var taskCompletionSource = new TaskCompletionSource<bool>();

        var compositeAnimation = new Animation();
        compositeAnimation.Add(0, 1, heightAnimation);
        compositeAnimation.Add(0, 1, translateAnimation);
        compositeAnimation.Add(0, 1, opacityAnimation);
        compositeAnimation.Add(0, 1, rotationAnimation);

        compositeAnimation.Commit(border, "CollapseAnimation", 16, 250, Easing.SpringIn,
            (v, c) => taskCompletionSource.TrySetResult(!c));

        await taskCompletionSource.Task;
        expandedLayout.IsVisible = false;
    }

    private StackLayout FindExpandedLayout(Border border)
    {
        if (border.Content is VerticalStackLayout mainLayout && mainLayout.Children.Count > 1)
        {
            if (mainLayout.Children[1] is StackLayout expandedLayout)
            {
                return expandedLayout;
            }
        }
        return null;
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnGlobalSwitchToggled(object sender, ToggledEventArgs e)
    {
        _globalSwitchIsToggled = e.Value;

        if (_globalSwitchIsToggled)
        {
            foreach (var day in DaysOfWeek)
            {
                day.IsEnabled = _globalSwitchIsToggled;
                day.SelectedTime = _globalSelectedTime;
            }
        }

        SaveSettingsAsync();
    }

    private void OnGlobalTimePickerChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time) && sender is TimePicker picker)
        {
            _globalSelectedTime = picker.Time;

            if (_globalSwitchIsToggled)
            {
                foreach (var day in DaysOfWeek)
                {
                    day.SelectedTime = _globalSelectedTime;
                }
            }

            SaveSettingsAsync();
        }
    }
}