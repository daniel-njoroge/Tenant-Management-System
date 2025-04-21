using MongoDB.Driver;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class SettingsView : UserControl
    {
        private readonly User LoggedInUser;
        private readonly MongoDBConnection _db;

        public SettingsView(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _db = new MongoDBConnection();
            fullNameLbl.Text = LoggedInUser.Fullname;
            statusLbl.Text = "";
        }

        private void editProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            settingsOverview.Visibility = Visibility.Collapsed;
            editProfile.Visibility = Visibility.Visible;
            emailTbx.Text = LoggedInUser.Email;
            statusLbl.Text = "";
        }

        private void confirmEditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(newPasswordTbx.Password))
            {
                statusLbl.Text = "New Password cannot be empty.";
                statusLbl.Foreground = Brushes.Red;
                return;
            }
            if (newPasswordTbx.Password.Length < 6)
            {
                statusLbl.Text = "Password must be at least 6 characters long.";
                statusLbl.Foreground = Brushes.Red;
                return;
            }
            if (newPasswordTbx.Password != confirmNewPasswordTbx.Password)
            {
                statusLbl.Text = "New Passwords do not match.";
                statusLbl.Foreground = Brushes.Red;
                return;
            }

            try
            {
                var user = _db.Users.Find(u => u.Email == emailTbx.Text && u.Password == previousPasswordTbx.Password).FirstOrDefault();
                if (user != null)
                {
                    var update = Builders<User>.Update.Set(u => u.Password, newPasswordTbx.Password);
                    _db.Users.UpdateOne(u => u.Id == user.Id, update);
                    statusLbl.Text = "Password changed successfully!";
                    statusLbl.Foreground = Brushes.Green;
                }
                else
                {
                    statusLbl.Text = "Wrong previous password.";
                    statusLbl.Foreground = Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error changing password: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }

        private void backToSettingsLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            editProfile.Visibility = Visibility.Collapsed;
            settingsOverview.Visibility = Visibility.Visible;
        }

        private void deleteAccountBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                var result = MessageBox.Show(
                    "Are you sure you want to delete your account? This action cannot be undone.",
                    "Confirm Account Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

            
                var apartmentFilter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(apartmentFilter).ToList();
                _db.Apartments.DeleteMany(apartmentFilter);

              
                var roomFilter = Builders<Room>.Filter.In(r => r.ApartmentNo, apartments.Select(a => a.ApartmentNo));
                _db.Rooms.DeleteMany(roomFilter);

                
                var tenantFilter = Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id);
                _db.Tenants.DeleteMany(tenantFilter);

                
                var userFilter = Builders<User>.Filter.Eq(u => u.Id, LoggedInUser.Id);
                _db.Users.DeleteOne(userFilter);

                statusLbl.Text = "Account and associated data deleted successfully!";
                statusLbl.Foreground = Brushes.Green;

                
                LoginPage loginPage = new LoginPage();
                Window.GetWindow(this).Close();
                loginPage.Show();
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error deleting account: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }
    }
}