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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public SubWindow(UserControl view)
        {
            InitializeComponent();

            // Set the datacontext to the previous view
            this.DataContext = view;

            // Set Width and height to the previous view's ones + 20
            this.Height = view.Height + 20;
            this.Width = view.Width + 20;
        }
    }
}
