using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ScottPlot;
using ScottPlot.Avalonia;
using ScottPlot.Plottables;

namespace TickChartControl
{
    public class TickChart : TemplatedControl
    {
        private AvaPlot? PART_PlotView;
        private DataLogger? _dataLogger;

        public static readonly StyledProperty<ChartTheme> PlotThemeProperty =
            AvaloniaProperty.Register<TickChart, ChartTheme>(nameof(PlotTheme), ChartTheme.Light);

        public ChartTheme PlotTheme
        {
            get => GetValue(PlotThemeProperty);
            set => SetValue(PlotThemeProperty, value);
        }

        public static readonly StyledProperty<int> MaxVisibleTicksProperty =
            AvaloniaProperty.Register<TickChart, int>(nameof(MaxVisibleTicks), 1000);

        public int MaxVisibleTicks
        {
            get => GetValue(MaxVisibleTicksProperty);
            set => SetValue(MaxVisibleTicksProperty, value);
        }

        static TickChart()
        {
            PlotThemeProperty.Changed.AddClassHandler<TickChart>((x, e) => x.OnThemeChanged());
        }

        private void OnThemeChanged()
        {
            ApplyTheme();
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            PART_PlotView = e.NameScope.Find<AvaPlot>("PART_PlotView");

            if (PART_PlotView != null)
            {
                _dataLogger = PART_PlotView.Plot.Add.DataLogger();
                _dataLogger.Color = ScottPlot.Colors.CornflowerBlue;

                ApplyTheme();
                PART_PlotView.Plot.Axes.AutoScale();
                PART_PlotView.Refresh();
            }
        }

        public void AddTick(double value)
        {
            if (PART_PlotView == null || _dataLogger == null) return;

            _dataLogger.Add(value);

            // Use the actual count of coordinates in the DataLogger's data source
            int currentLength = _dataLogger.Data.Coordinates.Count;

            PART_PlotView.Plot.Axes.SetLimitsX(currentLength - MaxVisibleTicks, currentLength);
            PART_PlotView.Plot.Axes.AutoScaleY();
            PART_PlotView.Refresh();
        }

        private void ApplyTheme()
        {
            if (PART_PlotView == null) return;

            // Use the SetStyle API with a new PlotStyle instance for dark/light themes
            if (PlotTheme == ChartTheme.Dark)
            {
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