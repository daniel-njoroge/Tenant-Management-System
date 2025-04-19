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

            apartmentsPnl.Visibility = Visibility.Collapsed;
            tenantsPnl.Visibility = Visibility.Collapsed;
            roomsPnl.Visibility = Visibility.Collapsed;
            settingsPnl.Visibility = Visibility.Collapsed;

        }

        private void apartmentsBtn_Click(object sender, RoutedEventArgs e)
        {
            
            tenantsPnl.Visibility = Visibility.Collapsed;
            roomsPnl.Visibility = Visibility.Collapsed;
            settingsPnl.Visibility = Visibility.Collapsed;
            apartmentsPnl.Visibility = Visibility.Visible;

        }

        private void tenantsBtn_Click(object sender, RoutedEventArgs e)
        {
            apartmentsPnl.Visibility = Visibility.Collapsed;
            roomsPnl.Visibility = Visibility.Collapsed;
            settingsPnl.Visibility = Visibility.Collapsed;
            tenantsPnl.Visibility = Visibility.Visible;
        }

        private void roomsBtn_Click(object sender, RoutedEventArgs e)
        {
            apartmentsPnl.Visibility = Visibility.Collapsed;
            tenantsPnl.Visibility = Visibility.Collapsed;
            settingsPnl.Visibility = Visibility.Collapsed;
            roomsPnl.Visibility = Visibility.Visible;

        }

        private void settingsBtn_Click(object sender, RoutedEventArgs e)
        {
            apartmentsPnl.Visibility = Visibility.Collapsed;
            tenantsPnl.Visibility = Visibility.Collapsed;
            roomsPnl.Visibility = Visibility.Collapsed;
            settingsPnl.Visibility = Visibility.Visible;
            settingsOverview.Visibility = Visibility.Visible;
            editProfile.Visibility = Visibility.Collapsed;

            fullNameLbl.Text = LoggedInUser.Fullname;
        }

        private void editProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            settingsOverview.Visibility = Visibility.Collapsed;
            editProfile.Visibility = Visibility.Visible;

            emailTbx.Text = LoggedInUser.Email;

            statusLbl.Text = " ";


        }

        private void confirmEditBtn_Click(object sender, RoutedEventArgs e)
        {

            
            if(string.IsNullOrEmpty(newPasswordTbx.Password))
            {
                statusLbl.Text = "New Password cannot be empty.";
                return;
            }
            if (newPasswordTbx.Password.Length < 6)
            {
                statusLbl.Text = "Password must be at least 6 characters long.";
            }
            if(newPasswordTbx.Password != confirmNewPasswordTbx.Password)
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
        private void logoutLinkTxt_Click(object sender, RoutedEventArgs e)
        {

            LoginPage loginPage = new LoginPage();

            this.Close();
            loginPage.Show();

        }
    }
}
