using Android.App;
using Android.Content;
using Microsoft.Maui.Handlers;
using Android.Content.Res;
using Android.Runtime;
using Android.Views;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;
using Plugin.LocalNotification.iOSOption;

namespace EmptyMauiApp;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    public override void OnCreate()
    {
        base.OnCreate();

        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });

        TimePickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList = ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.Background = null;
        });

#if IOS
        // Entry aláhúzás eltüntetése iOS-en (ez már megvan)
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
        });

        // TimePicker aláhúzás eltüntetése iOS-en
        TimePickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            // iOS-en a TimePicker (UIDatePicker) általában nem használ aláhúzást, de biztosítjuk az átlátszóságot
            handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
        });
#endif
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
