using Lab05.ModelView;
using System.Windows.Controls;

namespace Lab05.View
{
    /// <summary>
    /// Interaction logic for ProcessGridView.xaml
    /// </summary>
    public partial class ProcessGridView : UserControl
    {
        public ProcessGridView()
        {
            InitializeComponent();
            DataContext = new ProcessGridViewModel(); 
        }
    }
}
