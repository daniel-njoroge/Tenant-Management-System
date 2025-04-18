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

namespace Tenant_Management_System
{
    /// <summary>
    /// Interaction logic for MainApp.xaml
    /// </summary>
    /// 
    
    public partial class MainApp : Window
    {
        private User LoggedInUser;

        public MainApp(User user)
        {
            InitializeComponent();
            LoggedInUser = user;

            var lastname = LoggedInUser.Fullname.Split(' ')[1];
            welcomeLbl.Text += lastname ;
            welcomeLbl.Foreground = Brushes.Green;

        }

        private void apartmentsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tenantsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void roomsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
