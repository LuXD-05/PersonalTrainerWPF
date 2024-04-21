using PersonalTrainerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PersonalTrainerApp
{
    public partial class SubWindow : Window
    {
        public SubWindow(UserControl view)
        {
            InitializeComponent();

            // Imposto il datacontext della window alla view passata
            this.DataContext = view;

            this.Height = view.Height + 20;
            this.Width = view.Width + 20;
        }
    }
}
