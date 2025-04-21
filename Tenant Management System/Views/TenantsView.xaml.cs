using System.Windows;
using System.Windows.Controls;

namespace Tenant_Management_System.Views
{
    public partial class TenantsView : UserControl
    {
        public TenantsView()
        {
            InitializeComponent();
        }

        private void addNewTenantBtn_Click(object sender, RoutedEventArgs e)
        {
            tenantsOverview.Visibility = Visibility.Collapsed;
            addNewTenantPnl.Visibility = Visibility.Visible;
        }

        private void assignRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement assign room logic
        }

        private void addTenantBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add tenant logic (e.g., save to MongoDB)
        }

        private void backToTenantsLnk_Click(object sender, RoutedEventArgs e)
        {
            addNewTenantPnl.Visibility = Visibility.Collapsed;
            tenantsOverview.Visibility = Visibility.Visible;
        }
    }
}