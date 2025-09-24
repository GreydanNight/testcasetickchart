using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;

namespace TickChartControl
{
    /// <summary>
    /// Класс <see cref="TickChart"/> реализует пользовательский элемент управления
    /// для отображения тиковых данных в виде графика с использованием библиотеки ScottPlot.
    /// Поддерживает темы оформления и ограничение по количеству видимых тиков.
    /// </summary>
    public class TickChart : TemplatedControl
    {
        /// <summary>
        /// Ссылка на элемент графика из шаблона управления.
        /// </summary>
        private AvaPlot? PART_PlotView;

        /// <summary>
        /// Логгер данных для отображения входящих тиков на графике.
        /// </summary>
        private DataLogger? _dataLogger;

        /// <summary>
        /// Зарегистрированное свойство для выбора темы графика (светлая/тёмная).
        /// </summary>
        public static readonly StyledProperty<ChartTheme> PlotThemeProperty =
            AvaloniaProperty.Register<TickChart, ChartTheme>(nameof(PlotTheme), ChartTheme.Light);

        /// <summary>
        /// Тема оформления графика.
        /// </summary>
        public ChartTheme PlotTheme
        {
            get => GetValue(PlotThemeProperty);
            set => SetValue(PlotThemeProperty, value);
        }

        /// <summary>
        /// Зарегистрированное свойство для установки максимального количества видимых тиков.
        /// </summary>
        public static readonly StyledProperty<int> MaxVisibleTicksProperty =
            AvaloniaProperty.Register<TickChart, int>(nameof(MaxVisibleTicks), 1000);

        /// <summary>
        /// Максимальное количество видимых тиков на графике.
        /// </summary>
        public int MaxVisibleTicks
        {
            get => GetValue(MaxVisibleTicksProperty);
            set => SetValue(MaxVisibleTicksProperty, value);
        }

        /// <summary>
        /// Статический конструктор класса.
        /// Подписывается на событие изменения темы графика.
        /// </summary>
        static TickChart()
        {
            PlotThemeProperty.Changed.AddClassHandler<TickChart>((x, e) => x.OnThemeChanged());
        }

        /// <summary>
        /// Обработчик изменения темы графика.
        /// </summary>
        private void OnThemeChanged()
        {
            ApplyTheme();
        }

        /// <summary>
        /// Вызывается при применении шаблона управления.
        /// Инициализирует график и логгер данных.
        /// </summary>
        /// <param name="e">Аргументы применения шаблона.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            PART_PlotView = e.NameScope.Find<AvaPlot>("PART_PlotView");

            if (PART_PlotView != null)
            {
                // Инициализация логгера данных для отображения входящих значений
                _dataLogger = PART_PlotView.Plot.Add.DataLogger();
                _dataLogger.Color = ScottPlot.Colors.CornflowerBlue;

                // Применение темы и автоматический рескейл графика
                ApplyTheme();
                PART_PlotView.Plot.Axes.AutoScale();
                PART_PlotView.Refresh();
            }
        }

        /// <summary>
        /// Добавляет новый тик на график.
        /// Автоматически ограничивает диапазон X по количеству видимых тиков
        /// и выполняет авто-масштабирование оси Y.
        /// </summary>
        /// <param name="value">Значение тика.</param>
        public void AddTick(double value)
        {
            if (PART_PlotView == null || _dataLogger == null) return;

            _dataLogger.Add(value);
            int currentLength = _dataLogger.Data.Coordinates.Count;

            // Сдвиг окна X так, чтобы отображались только последние MaxVisibleTicks
            PART_PlotView.Plot.Axes.SetLimitsX(currentLength - MaxVisibleTicks, currentLength);

            // Автоматическое масштабирование по оси Y
            PART_PlotView.Plot.Axes.AutoScaleY();

            PART_PlotView.Refresh();
        }

        /// <summary>
        /// Применяет выбранную тему оформления (тёмную или светлую)
        /// к текущему экземпляру графика.
        /// </summary>
        private void ApplyTheme()
        {
            if (PART_PlotView == null) return;

            if (PlotTheme == ChartTheme.Dark)
            {
                // Настройки тёмной темы
                var darkStyle = new ScottPlot.PlotStyle
                {
                    FigureBackgroundColor = ScottPlot.Colors.Black,
                    DataBackgroundColor = ScottPlot.Colors.Black,
                    AxisColor = ScottPlot.Colors.White,
                    GridMajorLineColor = ScottPlot.Colors.Gray
                };
                PART_PlotView.Plot.SetStyle(darkStyle);

                if (_dataLogger is not null)
                {
                    _dataLogger.Color = ScottPlot.Colors.Cyan;
                }
            }
            else
            {
                // Настройки светлой темы
                var lightStyle = new ScottPlot.PlotStyle
                {
                    FigureBackgroundColor = ScottPlot.Colors.White,
                    DataBackgroundColor = ScottPlot.Colors.White,
                    AxisColor = ScottPlot.Colors.Black,
                    GridMajorLineColor = ScottPlot.Colors.LightGray
                };
                PART_PlotView.Plot.SetStyle(lightStyle);

                if (_dataLogger is not null)
                {
                    _dataLogger.Color = ScottPlot.Colors.CornflowerBlue;
                }
            }

            PART_PlotView.Refresh();
        }
    }
}
