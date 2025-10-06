
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EmptyMauiApp.Services;

namespace EmptyMauiApp.Views;

public class BasePage : ContentPage
{
    public BasePage()
    {
        ThemeManager.ThemeChanged += OnThemeChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshVisuals();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(object sender, int presetId)
    {
        RefreshVisuals();
    }

    protected virtual void RefreshVisuals()
    {
        if (Content is VisualElement visualElement)
        {
            RefreshElement(visualElement);
        }

        foreach (var key in new[] { "PrimaryColor", "SecondaryColor", "PrimaryTransparent",
                                   "SecondaryTransparent", "TitleColor",
                                   "ButtonTextColor", "ButtonColor" })
        {
            if (Application.Current.Resources.TryGetValue(key, out var value))
            {
                Resources[key] = value;
            }
        }

        if (this.Background is LinearGradientBrush gradientBrush)
        {
            gradientBrush.GradientStops[0].Color = (Color)Application.Current.Resources["PrimaryColor"];
            gradientBrush.GradientStops[1].Color = (Color)Application.Current.Resources["PrimaryColor"];
        }

        if (Content is VisualElement layout)
        {
            layout.InvalidateMeasure();
            //layout.InvalidateArrange(); //ez újrarendezi az elemeket
        }
    }

    private void RefreshElement(VisualElement element)
    {
        RefreshElementProperties(element);

        if (element is GraphicsView graphicsView)
        {
            graphicsView.Invalidate();
        }

        if (element.Background is LinearGradientBrush gradientBrush)
        {
            gradientBrush.GradientStops[0].Color = (Color)Application.Current.Resources["PrimaryColor"];
            gradientBrush.GradientStops[1].Color = (Color)Application.Current.Resources["PrimaryColor"];
        }

        if (element is IViewContainer<View> container)
        {
            foreach (var child in container.Children)
            {
                if (child is VisualElement childElement)
                {
                    RefreshElement(childElement);
                }
            }
        }
    }

    private void RefreshElementProperties(VisualElement element)
    {
        var propertiesToRefresh = new[]
        {
            VisualElement.BackgroundColorProperty,
            VisualElement.BackgroundProperty,
            Label.TextColorProperty,
            Button.TextColorProperty,
            Border.StrokeProperty,
            Border.BackgroundProperty,
            Entry.TextColorProperty,
            Image.SourceProperty,
        };

        foreach (var property in propertiesToRefresh)
        {
            if (element.IsSet(property))
            {
                var originalValue = element.GetValue(property);
                element.ClearValue(property);
                element.SetValue(property, originalValue);
            }
        }

        var themeProperties = new[]
        {
            (VisualElement.BackgroundColorProperty, "PrimaryColor"),
            (VisualElement.BackgroundColorProperty, "SecondaryColor"),
            (VisualElement.BackgroundColorProperty, "PrimaryTransparent"),
            (VisualElement.BackgroundColorProperty, "SecondaryTransparent"),
            (Label.TextColorProperty, "TitleColor"),
            (Label.TextColorProperty, "ButtonTextColor"),
            (Button.TextColorProperty, "ButtonColor"),
            (Border.StrokeProperty, "SecondaryTransparent"),
        };

        foreach (var (property, key) in themeProperties)
        {
            if (element.IsSet(property))
            {
                element.SetDynamicResource(property, key);
            }
        }
    }
}