using PersonalTrainerApp.ViewModels;
using PersonalTrainerApp.Views.SubViews;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // Sets the datacontext to the mainwindow's
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Handles the checked event of the radio buttons
        /// </summary>
        private void MenuRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // If the event is called by the radio button
            if (sender is RadioButton rbtn)
            {
                // Closes the detail if the opened sibview is Activities and opens another one
                if (((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).SelectedSubView is ActivitiesSubView && rbtn.Name != "btnActivities")
                    (((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).SelectedSubView as ActivitiesSubView).CloseActivityDetail(sender, null);
                
                // Calls the event to change subview
                ((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).RadioButtonChanged(sender, e);
            }
        }

        /// <summary>
        /// Handles the view's loading event
        /// </summary>
        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Checks the Home's radio button to load the default partial
            btnHome.IsChecked = true;
        }

        /// <summary>
        /// Handles the MouseDown event
        /// </summary>
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // If left mouse button clicked, DragMove
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }
    }
}
