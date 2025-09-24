using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using TickChartControl; // <-- Убедитесь, что эта строка есть

namespace UTS.AvaloniaUI.ComponentTask1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private TickChart? _chart;
        private bool _isTimerRunning = false;
        private IDisposable? _timerSub;
        private readonly Random _random = new();

        private int _maxTicks = 500;
        public int MaxTicks
        {
            get => _maxTicks;
            set => this.RaiseAndSetIfChanged(ref _maxTicks, value);
        }

        // FIX: Переименовали свойство в соответствии с изменениями в контроле
        private ChartTheme _selectedPlotTheme;
        public ChartTheme SelectedPlotTheme
        {
            get => _selectedPlotTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedPlotTheme, value);
        }

        // FIX: Переименовали свойство для ясности
        public Array PlotThemes => Enum.GetValues(typeof(ChartTheme));

        private string _buttonText = "Start";
        public string ButtonText
        {
            get => _buttonText;
            set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }

        public ICommand StartStopCommand { get; }

        public MainWindowViewModel()
        {
            StartStopCommand = ReactiveCommand.Create(ToggleTimer);
        }

        public void Initialize(TickChart chart)
        {
            _chart = chart;
        }

        private void ToggleTimer()
        {
            if (_isTimerRunning)
            {
                _timerSub?.Dispose();
                _isTimerRunning = false;
                ButtonText = "Start";
            }
            else
            {
                _timerSub = Observable.Interval(TimeSpan.FromMilliseconds(10))
                                      .ObserveOn(RxApp.MainThreadScheduler)
                                      .Subscribe(_ => AddNewTick());
                _isTimerRunning = true;
                ButtonText = "Stop";
            }
        }

        private void AddNewTick()
        {
            _chart?.AddTick(_random.NextDouble() * 100);
        }
    }
}