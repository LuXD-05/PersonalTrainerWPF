using PersonalTrainerApp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using PersonalTrainerApp.Models;
using System.Windows.Input;

namespace PersonalTrainerApp.Views.SubViews
{
    public partial class HomeSubView : UserControl
    {
        public HomeSubView()
        {
            InitializeComponent();

            // Sets the datacontext as the user
            DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Sets the specific datacontext for the graph
            lvcHome.DataContext = new ChartObject((Application.Current.MainWindow.DataContext as MainViewModel).User);
        }

        /// <summary>
        /// Handles the horizontal scroll for items in the dashboard
        /// </summary>
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }
    }
}
