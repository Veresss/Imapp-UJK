using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using EmptyMauiApp.Services;

namespace EmptyMauiApp.Views.CustomControls;

public class CircularProgressView : GraphicsView
{
    private float _pulseOpacity = 0f;
    private bool _isAnimationRunning = false;

    public CircularProgressView()
    {
        Drawable = new CircularProgressDrawable(this);
        ThemeManager.ThemeChanged += OnThemeChanged;
    }

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(CircularProgressView), 1.0, propertyChanged: OnProgressChanged);

    public static readonly BindableProperty IsTimerRunningProperty =
        BindableProperty.Create(nameof(IsTimerRunning), typeof(bool), typeof(CircularProgressView), false, propertyChanged: OnIsTimerRunningChanged);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public bool IsTimerRunning
    {
        get => (bool)GetValue(IsTimerRunningProperty);
        set => SetValue(IsTimerRunningProperty, value);
    }

    public Task<bool> ProgressTo(double targetProgress, uint duration, Easing easing)
    {
        var tcs = new TaskCompletionSource<bool>();

        this.Animate(
            name: "ProgressAnimation",
            callback: (value) => Progress = value,
            start: Progress,
            end: targetProgress,
            rate: 16,
            length: duration,
            easing: easing,
            finished: (v, c) => tcs.SetResult(c)
        );

        return tcs.Task;
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressView)bindable;
        if (view.Drawable is CircularProgressDrawable drawable)
        {
            drawable.Progress = (double)newValue;
            view.Invalidate();
        }
    }

    private static void OnIsTimerRunningChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressView)bindable;
        var isRunning = (bool)newValue;

        if (isRunning && !view._isAnimationRunning)
        {
            view.StartPulseAnimation();
        }
        else if (!isRunning && view._isAnimationRunning)
        {
            view.AbortAnimation("PulseBorder");
            view._isAnimationRunning = false;
            view.Invalidate();
        }
    }

    private void StartPulseAnimation()
    {
        _isAnimationRunning = true;
        this.Animate(
            name: "PulseBorder",
            callback: (value) =>
            {
                _pulseOpacity = (float)(Math.Sin(value * Math.PI) * 0.5);
                Invalidate();
            },
            start: 0,
            end: 2,
            rate: 16,
            length: 2000,
            easing: Easing.Linear,
            finished: (value, cancelled) =>
            {
                _isAnimationRunning = false;
                if (IsTimerRunning && !cancelled)
                    StartPulseAnimation();
            });
    }

    private void OnThemeChanged(object sender, int presetId)
    {
        Invalidate();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent == null)
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;
        }
    }

    private class CircularProgressDrawable : IDrawable
    {
        private readonly CircularProgressView _view;
        public double Progress { get; set; } = 1.0;

        public CircularProgressDrawable(CircularProgressView view)
        {
            _view = view;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var backgroundColor = Application.Current.Resources.TryGetValue("SecondaryTransparent", out var bgColor)
                ? (Color)bgColor
                : Colors.White.WithAlpha(0.31f);
            var progressColor = Application.Current.Resources.TryGetValue("SecondaryColor", out var progColor)
                ? (Color)progColor
                : Color.FromArgb("#FF8C8C");

            float centerX = dirtyRect.Center.X;
            float centerY = dirtyRect.Center.Y;
            float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 3;

            double safeProgress = Math.Clamp(Progress, 0, 1);

            canvas.StrokeColor = backgroundColor;
            canvas.StrokeSize = 20;
            canvas.DrawCircle(centerX, centerY, radius);

            canvas.StrokeColor = progressColor;
            canvas.StrokeSize = 8;
            canvas.DrawArc(
                centerX - radius, centerY - radius, 2 * radius, 2 * radius,
                startAngle: -90, endAngle: (float)(360 * safeProgress) - 90,
                clockwise: false, closed: false);

            if (_view.IsTimerRunning)
            {
                canvas.StrokeColor = progressColor.WithAlpha(_view._pulseOpacity);
                canvas.StrokeSize = 4;
                canvas.DrawCircle(centerX, centerY, radius - 14);
            }
        }
    }
}