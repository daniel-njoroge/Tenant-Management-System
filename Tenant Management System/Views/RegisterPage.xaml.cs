using MongoDB.Driver;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class RegisterPage : Window
    {
        private readonly MongoDBConnection _db;

        public RegisterPage()
        {
            InitializeComponent();
            _db = new MongoDBConnection();
            statusLbl.Text = " ";
        }

        private void registerBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fullname = fullNameTbx.Text;
                string email = emailTbx.Text;
                string password = passwordTbx.Password;
                string confirmPassword = confirmPasswordTbx.Password;

                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname))
                {
                    statusLbl.Text = "Email, password, and full name cannot be empty.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }
                if (password.Length < 6)
                {
                    statusLbl.Text = "Password must be at least 6 characters long.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }
                if (password != confirmPassword)
                {
                    statusLbl.Text = "Password and confirm password must match.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }
                if (!(email.Contains("@") && email.Contains(".")))
                {
                    statusLbl.Text = "Email is not valid.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                
                var existingEmail = _db.Users.Find(u => u.Email == email).FirstOrDefault();
                if (existingEmail != null)
                {
                    statusLbl.Text = "Email already registered!";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                
                var users = _db.Users.Find(FilterDefinition<User>.Empty).ToList();
                var nextUserID = users.Any() ? users.Max(u => u.UserID) + 1 : 1;

                
                var newUser = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserID = nextUserID,
                    Fullname = fullname,
                    Email = email,
                    Password = password
                };

                
                _db.Users.InsertOne(newUser);

                
                statusLbl.Text = "Registration successful!";
                statusLbl.Foreground = Brushes.Green;

                
                fullNameTbx.Text = "";
                emailTbx.Text = "";
                passwordTbx.Password = "";
                confirmPasswordTbx.Password = "";
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error during registration: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
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