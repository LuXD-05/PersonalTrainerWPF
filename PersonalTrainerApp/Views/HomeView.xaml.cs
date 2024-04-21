using PersonalTrainerApp.ViewModels;
using PersonalTrainerApp.Views.SubViews;
using System.Windows;
using System.Windows.Controls;

namespace PersonalTrainerApp.Views
{
    /// <summary>
    /// Logica di interazione per HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // Imposto il datacontext a quello della mainwindow
            DataContext = Application.Current.MainWindow.DataContext;
        }

        /// <summary>
        /// Gestisce l'evento Checked dei vari MenuRadioButtons
        /// </summary>
        private void MenuRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Se evento chiamato dal radiobutton
            if (sender is RadioButton rbtn)
            {
                // Chiudo il detail se la subview aperta è Activities e ne apro una diversa
                if (((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).SelectedSubView is ActivitiesSubView && rbtn.Name != "btnActivities")
                    (((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).SelectedSubView as ActivitiesSubView).CloseActivityDetail(sender, null);
                
                // Chiamo l'evento per cambiare subvierw
                ((this.DataContext as MainViewModel).SelectedViewModel as HomeViewModel).RadioButtonChanged(sender, e);
            }
        }

        /// <summary>
        /// Gestisce l'evento di caricamento della view
        /// </summary>
        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Checko il radio button Home per caricare la partial di default
            btnHome.IsChecked = true;
        }

        /// <summary>
        /// Gestisce l'evento MouseDown del controllo
        /// </summary>
        private void DragWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Se cliccato il mouseButton sinistro, esegui il DragMove della MainWindow
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }
    }
}
