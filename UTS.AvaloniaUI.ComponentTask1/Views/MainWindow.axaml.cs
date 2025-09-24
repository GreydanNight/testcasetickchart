using Avalonia.Controls;
using UTS.AvaloniaUI.ComponentTask1.ViewModels;

namespace UTS.AvaloniaUI.ComponentTask1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Этот код выполняется после того, как все элементы управления (включая LiveChart) созданы.
            this.Loaded += (sender, args) =>
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    // Передаем экземпляр нашего графика во ViewModel для управления им.
                    vm.Initialize(this.LiveChart);
                }
            };
        }
    }
}