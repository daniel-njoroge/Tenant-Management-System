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
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
            registererrorLbl.Text = " ";
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
           
            if(string.IsNullOrEmpty(emailTbx.Text) || string.IsNullOrEmpty(passwordTbx.Password) || string.IsNullOrEmpty(nameTbx.Text))
            {
                registererrorLbl.Text = "Email and password cannot be empty.";
                return;
            }
            if (passwordTbx.Password.Length < 6)
            {
                registererrorLbl.Text = "Password must be at least 6 characters long.";
                return;
            }
            if (passwordTbx.Password != confirmPasswordTbx.Password)
            {
                registererrorLbl.Text = "Password and confirm password must match.";
                return;
            }
            else
            {
                registererrorLbl.Text = "Success ";
                registererrorLbl.Foreground = Brushes.Green;
            }
            if(!(emailTbx.Text.Contains("@") && emailTbx.Text.Contains(".")))
            {
                registererrorLbl.Text = "Email is not valid";
                registererrorLbl.Foreground = Brushes.Red;
            }

        }

        private void loginLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();

            this.Close();
            loginPage.Show();

        }
    }
}
