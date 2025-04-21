using System.Windows;
using System.Windows.Controls;

namespace Tenant_Management_System.Views
{
    public partial class ApartmentsView : UserControl
    {
        public ApartmentsView()
        {
            InitializeComponent();
        }

        private void addNewApartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            apartmentsOverview.Visibility = Visibility.Collapsed;
            addNewApartmentPnl.Visibility = Visibility.Visible;
        }

        private void addApartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement add apartment logic (e.g., save to MongoDB)
        }

        private void backToAppartmentsLnk_Click(object sender, RoutedEventArgs e)
        {
            addNewApartmentPnl.Visibility = Visibility.Collapsed;
            apartmentsOverview.Visibility = Visibility.Visible;
        }
    }
}