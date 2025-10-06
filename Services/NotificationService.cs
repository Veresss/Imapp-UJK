using EmptyMauiApp.Models;
using Microsoft.Maui.Storage;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public static class NotificationService
{
    private const string NotificationsEnabledKey = "NotificationsEnabled";
    private const string DaysPreferencesKey = "WeeklyPlannerSettings";

    public static async Task UpdateNotificationsAsync()
    {
        bool notificationsEnabled = Preferences.Get(NotificationsEnabledKey, false); //false?
        if (!notificationsEnabled)
        {
            LocalNotificationCenter.Current.CancelAll();
            return;
        }

        var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
        if (!enabled)
        {
            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission();
            if (!granted)
            {
                Preferences.Set(NotificationsEnabledKey, false);
                //Opcionális: Alertet dobhatsz, de mivel szolgáltatás, ne dobj UI-t itt
                return;
            }
        }

        List<DayModel> days;
        if (Preferences.ContainsKey(DaysPreferencesKey))
        {
            var json = Preferences.Get(DaysPreferencesKey, string.Empty);
            days = !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<List<DayModel>>(json) : new List<DayModel>();
        }
        else
        {
            days = new List<DayModel>();
        }

        if (days.Count == 0) return;

        LocalNotificationCenter.Current.CancelAll();

        var dayOfWeekMap = new Dictionary<string, DayOfWeek>
        {
            { "Hétfő", DayOfWeek.Monday },
            { "Kedd", DayOfWeek.Tuesday },
            { "Szerda", DayOfWeek.Wednesday },
            { "Csütörtök", DayOfWeek.Thursday },
            { "Péntek", DayOfWeek.Friday },
            { "Szombat", DayOfWeek.Saturday },
            { "Vasárnap", DayOfWeek.Sunday }
        };

        int notificationId = 1;
        foreach (var day in days)
        {
            if (!day.IsEnabled || !dayOfWeekMap.TryGetValue(day.DayName, out var targetDay)) continue;

            var now = DateTime.Now;
            int daysUntil = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;
            if (daysUntil == 0 && now.TimeOfDay > day.SelectedTime)
            {
                daysUntil = 7;
            }
            var nextNotify = now.Date.AddDays(daysUntil).Add(day.SelectedTime);

            var request = new NotificationRequest
            {
                NotificationId = notificationId++,
                Title = "Emlékeztető",
                Description = $"{day.DayName} - Időpont: {day.SelectedTime:hh\\:mm}",
                Subtitle = "Az app emlékeztetője",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = nextNotify,
                    RepeatType = NotificationRepeat.TimeInterval,
                    NotifyRepeatInterval = TimeSpan.FromDays(7)
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }
}