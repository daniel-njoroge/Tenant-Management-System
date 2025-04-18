using MongoDB.Driver;
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
    public partial class LoginPage : Window
    {
        public LoginPage()
        {   
            InitializeComponent();
            statusLbl.Text = " ";
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            var db = new MongoDBConnection();
            var enteredEmail = emailTbx.Text;
            var enteredPassword = passwordTbx.Password;


            if (string.IsNullOrEmpty(emailTbx.Text) || string.IsNullOrEmpty(passwordTbx.Password))
            {
                statusLbl.Text = "Email and password cannot be empty.";
                return;
            }
            if (passwordTbx.Password.Length < 6)
            {
                statusLbl.Text = "Password must be at least 6 characters long.";
            }
            if (!(emailTbx.Text.Contains("@") && emailTbx.Text.Contains(".")))
            {
                statusLbl.Text = "Email is not valid";
                statusLbl.Foreground = Brushes.Red;
            }

            else{
                var user = db.Users.Find(u => u.Email == enteredEmail && u.Password == enteredPassword).FirstOrDefault();

                if (user != null)
                {
                    statusLbl.Text = "Login successful!";
                    statusLbl.Foreground = Brushes.Green;
                    MainApp mainApp = new MainApp(user);
                    this.Close();
                    mainApp.Show();
                }
                else
                {
                    statusLbl.Text = "Invalid email or password.";
                }
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
