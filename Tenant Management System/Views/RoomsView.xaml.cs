using MongoDB.Driver;
using System.Windows;
using System;
using System.Windows.Controls;
using Tenant_Management_System.Models;
using System.Linq;

namespace Tenant_Management_System.Views
{
    public partial class RoomsView : UserControl
    {
        private readonly User LoggedInUser;
        private readonly MongoDBConnection _db;
        public RoomsView(User user)
        {
            InitializeComponent();
            InitializeComponent();
            LoggedInUser = user;
            _db = new MongoDBConnection();
            LoadRooms();
        }

        private void LoadRooms()
        {
            try
            {
                var apartmentFilter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(apartmentFilter).ToList();
                var apartmentNos = apartments.Select(a => a.ApartmentNo).ToList();

                
                var roomFilter = Builders<Room>.Filter.In(r => r.ApartmentNo, apartmentNos);
                var rooms = _db.Rooms.Find(roomFilter).ToList();

                
                roomsDgr.ItemsSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void roomsDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (roomsDgr.SelectedItem is Room selectedRoom)
            {
                // do nothing for now
            }
        }

        private void reloadRoomsBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadRooms();
        }
    }
}