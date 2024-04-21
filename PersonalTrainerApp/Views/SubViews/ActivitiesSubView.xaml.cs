using Microsoft.Maps.MapControl.WPF;
using PersonalTrainerApp.Models;
using PersonalTrainerApp.Models.Controllers;
using PersonalTrainerApp.ViewModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PersonalTrainerApp.Views.SubViews
{
    /// <summary>
    /// Logica di interazione per ActivitiesSubView.xaml
    /// </summary>
    public partial class ActivitiesSubView : UserControl
    {
        private SubWindow _subWindow;
        private string _checkedRbtnName = string.Empty;

        public bool IsDetailOpened { get; set; } = false;
        public bool IsSubWindowOpened { get; set; } = false;

        public ActivitiesSubView()
        {
            InitializeComponent();

            // Imposto datacontext view a user
            this.DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Imposto datacontext dettaglio a null (x evitare errori binding)
            gDettaglio.DataContext = null;

            // Setto il checked radiobutton name iniziale
            _checkedRbtnName = "Todo";
        }

        #region Methods

        /// <summary>
        /// Cambia la visualizzazione delle attività
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Se il dettaglio e la subwindow non sono aperti
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // Controllo se evento mandato da un radiobutton checkato
                if (sender is RadioButton rbtn && rbtn.IsChecked == true && _checkedRbtnName != string.Empty)
                {
                    // Salvo il nome del radiobutton checkato
                    _checkedRbtnName = rbtn.Name.Remove(0, 4);

                    // Se ci sono start e end
                    if (dpStart.SelectedDate.HasValue && dpEnd.SelectedDate.HasValue && dpStart.SelectedDate <= dpEnd.SelectedDate)
                    {
                        DateTime start = dpStart.SelectedDate.Value;
                        DateTime end = dpEnd.SelectedDate.Value;

                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities;
                        else
                        {
                            // Prendo le attività nel range di date + se fatte o non fatte
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.DataFull >= start && x.DataFull < end.AddDays(1));
                        }
                    }
                    else
                    {
                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities;
                        else
                        {
                            // Prendo le attività se fatte o non fatte
                            lvActivities.ItemsSource = (this.DataContext as User).Activities.Where(x => _checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone);
                        }
                    } 
                }
            }
        }

        /// <summary>
        /// Filtra le attività in base alle date nei datepicker
        /// </summary>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // Se il dettaglio e la subwindow non sono aperti
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // Se ci sono start e end
                if (dpStart.SelectedDate.HasValue && dpEnd.SelectedDate.HasValue)
                {
                    DateTime start = dpStart.SelectedDate.Value;
                    DateTime end = dpEnd.SelectedDate.Value;

                    // Se data start <= data end
                    if (dpStart.SelectedDate <= dpEnd.SelectedDate)
                    {
                        // Prendo le attività (in base al checkedRadioButtonName) e comprese nel range di date
                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.DataFull >= start && x.DataFull < end.AddDays(1));
                        else
                        {
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.DataFull >= start && x.DataFull < end.AddDays(1));
                        }
                    }
                    else
                        MessageBox.Show("Data inizio o fine invalide", "GilTrainer", MessageBoxButton.OK, MessageBoxImage.Warning); 
                }
            }
        }

        /// <summary>
        /// Apre la finestra di creazione di un'attività
        /// </summary>
        private void NewActivity(object sender, RoutedEventArgs e)
        {
            // Se il dettaglio e la subwindow non sono aperti
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // Istanzio subwindow con view desiderata e attività
                _subWindow = new SubWindow(new NewActivity(this.DataContext as User));

                // Aggiungo handler per quando si chiude la subwindow
                _subWindow.Closed += SubWindow_Closed;

                // Mostro subwindow
                _subWindow.Show();

                // Setto variabile subwindow aperta
                IsSubWindowOpened = true;
            }
        }

        /// <summary>
        /// Gestisce la check della checkbox e setta se un'attività è completa o no
        /// </summary>
        private void CheckActivity(object sender, RoutedEventArgs e)
        {
            // Se la subwindow non è aperta
            if (!IsSubWindowOpened)
            {
                // Se evento è generato da checkbox ed è != null
                if (sender != null && sender is CheckBox cb)
                {
                    // Provo a ottenere il ListViewItem della cb
                    var item = GetAncestorOfType<ListViewItem>(cb);
                    if (item != null && item is ListViewItem)
                    {
                        // Provo a ottenere l'attività
                        var activity = item.Content;
                        if (activity != null && activity is Activity a)
                        {
                            // Ottengo utenti in db
                            var users = FileManager.GetUsers();

                            // Ottengo l'attività in questione dell'utente in questione e le assegno lo stato della checkbox
                            users.Single(x => x.Username == (this.DataContext as User).Username).Activities.Single(x => x.Nome == a.Nome).IsDone = a.IsDone;

                            // Aggiorno db
                            FileManager.UpdateDb(users);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Apre la finestra di modifica di un'attività
        /// </summary>
        private void EditActivity(object sender, RoutedEventArgs e)
        {
            // Se la subwindow non è aperta
            if (!IsSubWindowOpened)
            {
                // Provo a ottenere il ListViewItem
                var item = GetAncestorOfType<ListViewItem>(sender as Button);
                if (item != null && item is ListViewItem)
                {
                    // Provo a ottenere l'attività
                    var activity = item.Content;
                    if (activity != null && activity is Activity a)
                    {
                        // Istanzio subwindow con view desiderata e attività
                        _subWindow = new SubWindow(new EditActivity(a, this.DataContext as User));

                        // Aggiungo handler per quando si chiude la subwindow
                        _subWindow.Closed += SubWindow_Closed;

                        // Mostro subwindow
                        _subWindow.Show();

                        // Setto variabile subwindow aperta
                        IsSubWindowOpened = true;
                    }
                }
                else if ((sender as Button).Name == "btnEdit")
                {
                    // Istanzio subwindow con view desiderata e attività
                    _subWindow = new SubWindow(new EditActivity(gDettaglio.DataContext as Activity, this.DataContext as User));

                    // Aggiungo handler per quando si chiude la subwindow
                    _subWindow.Closed += SubWindow_Closed;

                    // Mostro subwindow
                    _subWindow.Show();

                    // Setto variabile subwindow aperta
                    IsSubWindowOpened = true;
                } 
            }
        }

        /// <summary>
        /// Elimina un'attività
        /// </summary>
        private void DeleteActivity(object sender, RoutedEventArgs e)
        {
            // Se la subwindow non è aperta
            if (!IsSubWindowOpened)
            {
                // Provo a ottenere il ListViewItem (caso elimina da ListView)
                var item = GetAncestorOfType<ListViewItem>(sender as Button);
                if (item != null && item is ListViewItem)
                {
                    // Provo a ottenere l'attività
                    var a = item.Content;
                    if (a != null && a is Activity)
                    {
                        // Elimino l'attività (Model)
                        (this.DataContext as User).Activities.Remove(a as Activity);

                        // Ottengo gli utenti a db
                        var users = FileManager.GetUsers();

                        // Ottengo l'indice dell'utente in questione nella lista
                        int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                        // Elimino l'attività (List)
                        users[i].Activities.Remove(users[i].Activities.Single(x => x.Nome == (a as Activity).Nome));

                        // Aggiorno db
                        FileManager.UpdateDb(users);
                    }
                }
                // Vedo se cliccato il btnEdit (caso elimina da Dettaglio)
                else if ((sender as Button).Name == "btnDelete")
                {
                    // Elimino l'attività (Model)
                    (this.DataContext as User).Activities.Remove(gDettaglio.DataContext as Activity);

                    // Ottengo gli utenti a db
                    var users = FileManager.GetUsers();

                    // Ottengo l'indice dell'utente in questione nella lista
                    int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                    // Elimino l'attività (List)
                    users[i].Activities.Remove(users[i].Activities.Single(x => x.Nome == (gDettaglio.DataContext as Activity).Nome));

                    // Aggiorno db
                    FileManager.UpdateDb(users);
                }

                // Chiudo il dettaglio attività
                this.CloseActivityDetail(sender, null);
            }
        }

        /// <summary>
        /// Gestisce l'evento click del selectedItem della ListView aprendo la finestra di dettaglio
        /// </summary>
        private void OpenActivityDetail(object sender, MouseButtonEventArgs e)
        {
            // Se la subwindow non è aperta
            if (!IsSubWindowOpened)
            {
                // Se ho cliccato su un bottone nell'item invece che sull'item
                if (GetAncestorOfType<Button>(e.OriginalSource as FrameworkElement) != null)
                    return;

                // Controllo se item selezionato e != null
                var item = sender as ListViewItem;
                if (item != null && item.IsSelected)
                {
                    // Ottengo il contenuto (attività) dell'item se c'è 
                    if (item.HasContent && item.Content is Activity a)
                    {
                        // Imposto datacontext sezione dettaglio all'attività selezionata
                        gDettaglio.DataContext = a;

                        // Imposto location e zoom
                        SetMap(a.Coordinate);

                        // Apro colonna dettaglio
                        cDetail.Width = new GridLength(1, GridUnitType.Star);

                        // Setto variabile dettaglio aperto
                        IsDetailOpened = true;
                    }
                } 
            }
        }

        /// <summary>
        /// Chiude la colonna contenente il dettaglio dell'attività selezionata
        /// </summary>
        public void CloseActivityDetail(object sender, MouseButtonEventArgs e)
        {
            // Se la subwindow non è aperta
            if (!IsSubWindowOpened)
            {
                // Chiudo colonna dettaglio
                cDetail.Width = new GridLength(0);

                // Tolgo il datacontext (risparmio memoria perche non renderizza map?)
                gDettaglio.DataContext = null;

                // Setto variabile dettaglio chiuso
                IsDetailOpened = false; 
            }
        }

        /// <summary>
        /// Sorta gli elementi in base all'header cliccato
        /// </summary>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // Ottengo la gridviewcolumn cliccata
            var gvc = e.OriginalSource as GridViewColumnHeader;

            // Tolgo sort precedenti
            lvActivities.Items.SortDescriptions.Clear();

            // Sorto in base al nome di colonna
            switch (gvc?.Content.ToString())
            {
                case "Nome":
                    // Ordina per il nome
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Nome", ListSortDirection.Ascending));
                    break;
                case "Data e ora":
                    // Ordina per la data e ora
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("DataOra", ListSortDirection.Ascending));
                    break;
                case "Lunghezza":
                    // Ordina per la lunghezza
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Lunghezza", ListSortDirection.Ascending));
                    break;
                case "Calorie":
                    // Ordina per le calorie
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Calorie", ListSortDirection.Ascending));
                    break;
                case "Tipo":
                    // Ordina per il tipo
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Tipo", ListSortDirection.Ascending));
                    break;
                default:
                    return;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ottiene il parent del controllo specificato (generic)
        /// </summary>
        /// <typeparam name="T">Tipo del controllo specificato</typeparam>
        /// <param name="child">Controllo di cui ottenere il parent</param>
        private T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent != null && !(parent is T))
                return (T)GetAncestorOfType<T>((FrameworkElement)parent);
            return (T)parent;
        }

        /// <summary>
        /// Metodo che gestisce la chiusura della subwindow, aggiornando i controlli da aggiornare
        /// </summary>
        private void SubWindow_Closed(object sender, EventArgs e)
        {
            // Rimuovo handler
            _subWindow.Closed -= SubWindow_Closed;

            // Setto variabile subwindow chiusa
            IsSubWindowOpened = false;

            // Se datacontext != null e is activity
            if (gDettaglio.DataContext != null && gDettaglio.DataContext is Activity a)
            {
                // Aggiorno mappa
                SetMap(a.Coordinate);
            }
        }

        /// <summary>
        /// Setta centro, pushpin e zoom della mappa
        /// </summary>
        /// <param name="l">Punto del pushpin</param>
        private void SetMap(Location l)
        {
            mapControl.Children.Clear();
            mapControl.Center = l;
            mapControl.Children.Add(new Pushpin() { Location = l });
            mapControl.ZoomLevel = 7;
        }

        #endregion
    }
}
