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
    public partial class ActivitiesSubView : UserControl
    {
        private SubWindow _subWindow;
        private string _checkedRbtnName = string.Empty;

        public bool IsDetailOpened { get; set; } = false;
        public bool IsSubWindowOpened { get; set; } = false;

        public ActivitiesSubView()
        {
            InitializeComponent();

            // Sets the datacontext as the user
            this.DataContext = (Application.Current.MainWindow.DataContext as MainViewModel).User;

            // Sets the detail grid datacontext at null (to avoid binding errors)
            gDettaglio.DataContext = null;

            // Closes the activity detail
            CloseActivityDetail(null, null);

            // Sets the initial checked radio button name
            _checkedRbtnName = "Todo";
        }

        #region Methods

        /// <summary>
        /// Changes activities view (render)
        /// </summary>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // If detail and subwindow arent opened
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // Checks if the event is sent by a checked radio button
                if (sender is RadioButton rbtn && rbtn.IsChecked == true && _checkedRbtnName != string.Empty)
                {
                    // Saves the checked radio button name
                    _checkedRbtnName = rbtn.Name.Remove(0, 4);

                    // If there are start and end
                    if (dpStart.SelectedDate.HasValue && dpEnd.SelectedDate.HasValue && dpStart.SelectedDate <= dpEnd.SelectedDate)
                    {
                        DateTime start = dpStart.SelectedDate.Value;
                        DateTime end = dpEnd.SelectedDate.Value;

                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities;
                        else
                        {
                            // Gets the activities in the date range + if done or not
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.FullDate >= start && x.FullDate < end.AddDays(1));
                        }
                    }
                    else
                    {
                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities;
                        else
                        {
                            // Gets the activities if done or not
                            lvActivities.ItemsSource = (this.DataContext as User).Activities.Where(x => _checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone);
                        }
                    } 
                }
            }
        }

        /// <summary>
        /// Filters the activities based on the names in the datepicker
        /// </summary>
        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            // If detail and subwindow arent opened
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // If there are start and end
                if (dpStart.SelectedDate.HasValue && dpEnd.SelectedDate.HasValue)
                {
                    DateTime start = dpStart.SelectedDate.Value;
                    DateTime end = dpEnd.SelectedDate.Value;

                    // If startdate <= enddate
                    if (dpStart.SelectedDate <= dpEnd.SelectedDate)
                    {
                        // Gets the activities (based on the checkedRadioButtonName) And between the date range
                        if (_checkedRbtnName == "All")
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.FullDate >= start && x.FullDate < end.AddDays(1));
                        else
                        {
                            lvActivities.ItemsSource = (this.DataContext as User).Activities
                                .Where(x => (_checkedRbtnName == "Todo" ? !x.IsDone : x.IsDone) && x.FullDate >= start && x.FullDate < end.AddDays(1));
                        }
                    }
                    else
                        MessageBox.Show("Data inizio o fine invalide", "GilTrainer", MessageBoxButton.OK, MessageBoxImage.Warning); 
                }
            }
        }

        /// <summary>
        /// Opens the window to create a new activity
        /// </summary>
        private void NewActivity(object sender, RoutedEventArgs e)
        {
            // If detail and subwindow arent opened
            if (!IsDetailOpened && !IsSubWindowOpened)
            {
                // Instances the subwindow with desired view and activity
                _subWindow = new SubWindow(new NewActivity(this.DataContext as User));

                // Adds handler for when the subwindow closes
                _subWindow.Closed += SubWindow_Closed;

                // Shows the subwindow
                _subWindow.Show();

                // Sets subwindow variable true
                IsSubWindowOpened = true;
            }
        }

        /// <summary>
        /// Handles checkbox's check and sets if an activity is completed or not
        /// </summary>
        private void CheckActivity(object sender, RoutedEventArgs e)
        {
            // If subwindow isn't opened
            if (!IsSubWindowOpened)
            {
                // If the event is generated from a checkbox and is != null
                if (sender != null && sender is CheckBox cb)
                {
                    // Tries to obtain the ListviewItem of the cb
                    var item = GetAncestorOfType<ListViewItem>(cb);
                    if (item != null && item is ListViewItem)
                    {
                        // Tries to get the activity
                        var activity = item.Content;
                        if (activity != null && activity is Activity a)
                        {
                            // Gets users in the db
                            var users = FileManager.GetUsers();

                            // Gets the the user's activity and assigns to it the checkbox status
                            users.Single(x => x.Username == (this.DataContext as User).Username).Activities.Single(x => x.Name == a.Name).IsDone = a.IsDone;

                            // Updates db
                            FileManager.UpdateDb(users);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens the activity edit window
        /// </summary>
        private void EditActivity(object sender, RoutedEventArgs e)
        {
            // If subwindow isn't opened
            if (!IsSubWindowOpened)
            {
                // Tries to get the ListViewItem
                var item = GetAncestorOfType<ListViewItem>(sender as Button);
                if (item != null && item is ListViewItem)
                {
                    // Tries to get the activity
                    var activity = item.Content;
                    if (activity != null && activity is Activity a)
                    {
                        // Instances the subwindow with desired view and activity
                        _subWindow = new SubWindow(new EditActivity(a, this.DataContext as User));

                        // Adds handler for when the subwindow closes
                        _subWindow.Closed += SubWindow_Closed;

                        // Shows the subwindow
                        _subWindow.Show();

                        // Sets subwindow variable true
                        IsSubWindowOpened = true;
                    }
                }
                else if ((sender as Button).Name == "btnEdit")
                {
                    // Instances the subwindow with desired view and activity
                    _subWindow = new SubWindow(new EditActivity(gDettaglio.DataContext as Activity, this.DataContext as User));

                    // Adds handler for when the subwindow closes
                    _subWindow.Closed += SubWindow_Closed;

                    // Shows the subwindow
                    _subWindow.Show();

                    // Sets subwindow variable true
                    IsSubWindowOpened = true;
                } 
            }
        }

        /// <summary>
        /// Deletes an activity
        /// </summary>
        private void DeleteActivity(object sender, RoutedEventArgs e)
        {
            // If subwindow isn't opened
            if (!IsSubWindowOpened)
            {
                // Tries to get the ListViewItem (caso elimina da ListView)
                var item = GetAncestorOfType<ListViewItem>(sender as Button);
                if (item != null && item is ListViewItem)
                {
                    // Tries to get the activity
                    var a = item.Content;
                    if (a != null && a is Activity)
                    {
                        // Deletes activity (Model)
                        (this.DataContext as User).Activities.Remove(a as Activity);

                        // Gets the users from db
                        var users = FileManager.GetUsers();

                        // Gets the user's index in the list
                        int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                        // Deletes activity (List)
                        users[i].Activities.Remove(users[i].Activities.Single(x => x.Name == (a as Activity).Name));

                        // Updates db
                        FileManager.UpdateDb(users);
                    }
                }
                // If clicked btnEdit (case delete from Detail)
                else if ((sender as Button).Name == "btnDelete")
                {
                    // Deletes activity (Model)
                    (this.DataContext as User).Activities.Remove(gDettaglio.DataContext as Activity);

                    // Gets the users from db
                    var users = FileManager.GetUsers();

                    // Gets the user's index in the list
                    int i = users.IndexOf(users.Single(x => x.Username == (this.DataContext as User).Username));

                    // Deletes activity (List)
                    users[i].Activities.Remove(users[i].Activities.Single(x => x.Name == (gDettaglio.DataContext as Activity).Name));

                    // Updates db
                    FileManager.UpdateDb(users);
                }

                // Closes activity detail
                this.CloseActivityDetail(sender, null);
            }
        }

        /// <summary>
        /// Handles the ListView's selectedItem's click event opening the detail window
        /// </summary>
        private void OpenActivityDetail(object sender, MouseButtonEventArgs e)
        {
            // If subwindow isn't opened
            if (!IsSubWindowOpened)
            {
                // If item's btn was clicked instead of the item itself
                if (GetAncestorOfType<Button>(e.OriginalSource as FrameworkElement) != null)
                    return;

                // Checks if item is selected and != null
                var item = sender as ListViewItem;
                if (item != null && item.IsSelected)
                {
                    // Getse the (activity) content of the item if has content
                    if (item.HasContent && item.Content is Activity a)
                    {
                        // Sets detail section datacontext to the selected activity
                        gDettaglio.DataContext = a;

                        // Sets location and zoom
                        SetMap(a.Coordinate);

                        // Opens detail column
                        cDetail.Width = new GridLength(1, GridUnitType.Star);

                        // Sets opened detail variable to true
                        IsDetailOpened = true;
                    }
                } 
            }
        }

        /// <summary>
        /// Closes the column containing the selected activity detail
        /// </summary>
        public void CloseActivityDetail(object sender, MouseButtonEventArgs e)
        {
            // If subwindow isn't opened
            if (!IsSubWindowOpened)
            {
                // Closes detail column
                cDetail.Width = new GridLength(0);

                // Removes the datacontext
                gDettaglio.DataContext = null;

                // Sets variable opened detail to false
                IsDetailOpened = false; 
            }
        }

        /// <summary>
        /// Sorts elements based on the clicked header
        /// </summary>
        private void GridViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            // Gets the clicked gridviewcolumn
            var gvc = e.OriginalSource as GridViewColumnHeader;

            // Remover old sorts
            lvActivities.Items.SortDescriptions.Clear();

            // Sorts based on the column name
            switch (gvc?.Content.ToString())
            {
                case "Name":
                    // Orders by name
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                    break;
                case "Data e ora":
                    // Orders by date and time
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("DateTimeString", ListSortDirection.Ascending));
                    break;
                case "Length":
                    // Orders by length
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Length", ListSortDirection.Ascending));
                    break;
                case "Calories":
                    // Orders by calories
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Calories", ListSortDirection.Ascending));
                    break;
                case "Type":
                    // Orders by type
                    lvActivities.Items.SortDescriptions.Add(new SortDescription("Type", ListSortDirection.Ascending));
                    break;
                default:
                    return;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the specified control's parent (generic)
        /// </summary>
        /// <typeparam name="T">Specified control's Type</typeparam>
        /// <param name="child">Control of which obtain the parent</param>
        private T GetAncestorOfType<T>(FrameworkElement child) where T : FrameworkElement
        {
            var parent = VisualTreeHelper.GetParent(child);
            if (parent != null && !(parent is T))
                return (T)GetAncestorOfType<T>((FrameworkElement)parent);
            return (T)parent;
        }

        /// <summary>
        /// Handles subwindow closed, updating things
        /// </summary>
        private void SubWindow_Closed(object sender, EventArgs e)
        {
            // Removes handler
            _subWindow.Closed -= SubWindow_Closed;

            // Sets variable opened subwindow to false
            IsSubWindowOpened = false;

            // if datacontext != null && is activity
            if (gDettaglio.DataContext != null && gDettaglio.DataContext is Activity a)
            {
                // Updates map
                SetMap(a.Coordinate);
            }
        }

        /// <summary>
        /// Sets center, pushpin and zoom on the map
        /// </summary>
        /// <param name="l">Pushpin point</param>
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
