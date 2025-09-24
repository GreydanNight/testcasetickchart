using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using TickChartControl;

namespace UTS.AvaloniaUI.ComponentTask1.ViewModels
{
    /// <summary>
    /// ViewModel для главного окна приложения.
    /// Управляет логикой запуска/остановки генерации тиков
    /// и взаимодействует с компонентом <see cref="TickChart"/>.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// Ссылка на элемент управления графиком.
        /// </summary>
        private TickChart? _chart;

        /// <summary>
        /// Флаг, указывающий, запущен ли таймер генерации тиков.
        /// </summary>
        private bool _isTimerRunning = false;

        /// <summary>
        /// Подписка на таймер (используется для отмены при остановке).
        /// </summary>
        private IDisposable? _timerSub;

        /// <summary>
        /// Генератор случайных значений для симуляции входящих тиков.
        /// </summary>
        private readonly Random _random = new();

        /// <summary>
        /// Максимальное количество тиков, отображаемых на графике.
        /// </summary>
        private int _maxTicks = 500;
        public int MaxTicks
        {
            get => _maxTicks;
            set => this.RaiseAndSetIfChanged(ref _maxTicks, value);
        }

        /// <summary>
        /// Выбранная тема графика.
        /// </summary>
        private ChartTheme _selectedPlotTheme;
        public ChartTheme SelectedPlotTheme
        {
            get => _selectedPlotTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedPlotTheme, value);
        }

        /// <summary>
        /// Список доступных тем графика (светлая/тёмная).
        /// </summary>
        public Array PlotThemes => Enum.GetValues(typeof(ChartTheme));

        /// <summary>
        /// Текст на кнопке запуска/остановки таймера.
        /// </summary>
        private string _buttonText = "Start";
        public string ButtonText
        {
            get => _buttonText;
            set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }

        /// <summary>
        /// Команда запуска/остановки генерации тиков.
        /// </summary>
        public ICommand StartStopCommand { get; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// Инициализирует команду <see cref="StartStopCommand"/>.
        /// </summary>
        public MainWindowViewModel()
        {
            StartStopCommand = ReactiveCommand.Create(ToggleTimer);
        }

        /// <summary>
        /// Инициализирует ViewModel, устанавливая ссылку на график.
        /// </summary>
        /// <param name="chart">Компонент графика.</param>
        public void Initialize(TickChart chart)
        {
            _chart = chart;
        }

        /// <summary>
        /// Переключает состояние таймера:
        /// запускает или останавливает генерацию случайных тиков.
        /// </summary>
        private void ToggleTimer()
        {
            if (_isTimerRunning)
            {
                // Остановка генерации тиков
                _timerSub?.Dispose();
                _isTimerRunning = false;
                ButtonText = "Start";
            }
            else
            {
                // Запуск генерации тиков каждые 10 мс
                _timerSub = Observable.Interval(TimeSpan.FromMilliseconds(10))
                                      .ObserveOn(RxApp.MainThreadScheduler)
                                      .Subscribe(_ => AddNewTick());
                _isTimerRunning = true;
                ButtonText = "Stop";
            }
        }

        /// <summary>
        /// Добавляет новый случайный тик на график.
        /// </summary>
        private void AddNewTick()
        {
            _chart?.AddTick(_random.NextDouble() * 100);
        }
    }
}
