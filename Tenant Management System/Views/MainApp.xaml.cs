using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class MainApp : Window
    {
        private User LoggedInUser;
        private ApartmentsView apartmentsView;
        private TenantsView tenantsView;
        private RoomsView roomsView;
        private PaymentsView paymentsView;
        private SettingsView settingsView;

        public MainApp(User user)
        {
            InitializeComponent();
            LoggedInUser = user;

            // Initialize welcome label
            var lastname = LoggedInUser.Fullname.Split(' ')[1];
            welcomeLbl.Text += lastname;
            welcomeLbl.Foreground = Brushes.Green;

            // Initialize UserControls
            apartmentsView = new ApartmentsView(LoggedInUser);
            tenantsView = new TenantsView(LoggedInUser);
            roomsView = new RoomsView(LoggedInUser);
            paymentsView = new PaymentsView(LoggedInUser);
            settingsView = new SettingsView(LoggedInUser);

            // Set default view
            MainContent.Content = apartmentsView;
        }

        private void apartmentsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = apartmentsView;
        }

        private void tenantsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = tenantsView;
        }

        private void roomsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = roomsView;
        }

        private void paymentsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = paymentsView;
        }

        private void settingsTabBtn_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = settingsView;
        }

        private void logoutLinkTxt_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            this.Close();
            loginPage.Show();
        }
    }
}