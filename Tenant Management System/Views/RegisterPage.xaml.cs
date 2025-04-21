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
using Tenant_Management_System.Views;

namespace Tenant_Management_System.Views
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Window
    {
        public RegisterPage()
        {
            InitializeComponent();
            statusLbl.Text = " ";
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            var db = new MongoDBConnection();
            string fullname = fullNameTbx.Text;
            string email = emailTbx.Text;
            string password = passwordTbx.Password;
            string confirmPassword = confirmPasswordTbx.Password;


            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname))
            {
                statusLbl.Text = "Email and password cannot be empty.";
                return;
            }
            if (passwordTbx.Password.Length < 6)
            {
                statusLbl.Text = "Password must be at least 6 characters long.";
                return;
            }
            if (passwordTbx.Password != confirmPasswordTbx.Password)
            {
                statusLbl.Text = "Password and confirm password must match.";
                return;
            }
            if (!(emailTbx.Text.Contains("@") && emailTbx.Text.Contains(".")))
            {
                statusLbl.Text = "Email is not valid";
            }
            else
            {
                var existingEmail = db.Users.Find(u => u.Email == email).FirstOrDefault();
                if (existingEmail != null)
                {
                    statusLbl.Text = "Email Already Registered!";
                    return;
                }

                var newUser = new User
                {
                    Fullname = fullname,
                    Email = email,
                    Password = password
                };


                db.Users.InsertOne(newUser);

                statusLbl.Text = "Registration successful!";
                statusLbl.Foreground = Brushes.Green;


                fullname = "";
                email = "";
                password = "";
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
