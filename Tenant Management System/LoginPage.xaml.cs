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
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    /// this is code
    /// ok
    public partial class LoginPage : Window
    {
        public LoginPage()
        {   
            InitializeComponent();
            loginerrorLbl.Text = " ";
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(emailTbx.Text) || string.IsNullOrEmpty(passwordTbx.Password))
            {
                loginerrorLbl.Text = "Email and password cannot be empty.";
                return;
            }
            if (passwordTbx.Password.Length < 6)
            {
                loginerrorLbl.Text = "Password must be at least 6 characters long.";
            }
            else
            {
                loginerrorLbl.Text = " ";
            }
        }
        private void registerLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            
            RegisterPage registerPage = new RegisterPage();
            registerPage.Show();
            this.Close();

        }
    }
}
