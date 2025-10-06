using System.Timers;
using EmptyMauiApp.Views;

namespace EmptyMauiApp.Services;

public class TimerService : IDisposable
{
    private static TimerService _instance;
    public static TimerService Instance => _instance ??= new TimerService();

    private System.Timers.Timer _timer;
    private int _remainingSeconds;
    private int _totalSeconds;

    public event Action<int> Tick;
    public event Action Completed;

    public bool IsRunning => _timer != null;
    public int RemainingSeconds => _remainingSeconds;

    private TimerService() { } //privi konstruktor singletonhoz

    public void Start(int totalSeconds)
    {
        if (IsRunning) Stop();

        _totalSeconds = totalSeconds;
        _remainingSeconds = totalSeconds;

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }

    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _remainingSeconds = 0;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _remainingSeconds--;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Tick?.Invoke(_remainingSeconds);
        });

        if (_remainingSeconds <= 0)
        {
            Stop();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Completed?.Invoke();
            });
        }
    }

    public void Dispose()
    {
        Stop();
    }
}