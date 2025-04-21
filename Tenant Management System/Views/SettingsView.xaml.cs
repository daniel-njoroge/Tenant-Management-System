using MongoDB.Driver;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class SettingsView : UserControl
    {
        private User LoggedInUser;

        public SettingsView(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            fullNameLbl.Text = LoggedInUser.Fullname;
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
                return;
            }
            if (newPasswordTbx.Password.Length < 6)
            {
                statusLbl.Text = "Password must be at least 6 characters long.";
            }
            if (newPasswordTbx.Password != confirmNewPasswordTbx.Password)
            {
                statusLbl.Text = "New Passwords do not match.";
            }
            else
            {
                var db = new MongoDBConnection();
                var user = db.Users.Find(u => u.Email == emailTbx.Text && u.Password == previousPasswordTbx.Password).FirstOrDefault();
                if (user != null)
                {
                    var update = Builders<User>.Update.Set(u => u.Password, newPasswordTbx.Password);
                    db.Users.UpdateOne(u => u.Id == user.Id, update);
                    statusLbl.Text = "Password Changed successfully!";
                    statusLbl.Foreground = Brushes.Green;
                }
                else
                {
                    statusLbl.Text = "Wrong Previous Password.";
                    statusLbl.Foreground = Brushes.Red;
                }
            }
        }

        private void backToSettingsLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            editProfile.Visibility = Visibility.Collapsed;
            settingsOverview.Visibility = Visibility.Visible;
        }
    }
}