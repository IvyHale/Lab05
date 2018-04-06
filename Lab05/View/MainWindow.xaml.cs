using Lab05.View;
using System.Windows;

namespace Lab05
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessGridView _processGridView;
        public MainWindow()
        {
            InitializeComponent();
            ShowProcessGridView();
        }
        private void ShowProcessGridView()
        {

            if (_processGridView == null)
            {
                _processGridView = new ProcessGridView();
            }
            ShowView(_processGridView);
        }

        private void ShowView(UIElement element)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(element);
        }
    }
}
