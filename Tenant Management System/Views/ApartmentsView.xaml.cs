using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class ApartmentsView : UserControl
    {
        private readonly User LoggedInUser;
        private readonly MongoDBConnection _db;

        public ApartmentsView(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _db = new MongoDBConnection();
            LoadApartments();
        }

        private void LoadApartments()
        {
            try
            {
                
                var filter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(filter).ToList();
                apartmentsDgr.ItemsSource = apartments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading apartments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void apartmentsDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (apartmentsDgr.SelectedItem is Apartment selectedApartment)
            {
                selectedApartmentNoTbx.Text = selectedApartment.ApartmentNo;
                selectedApartmentNameTbx.Text = selectedApartment.ApartmentName;
                selectedApartmentPricePerRoomTbx.Text = selectedApartment.PricePerRoom.ToString();
            }
            else
            {
                
                selectedApartmentNoTbx.Text = "";
                selectedApartmentNameTbx.Text = "";
                selectedApartmentPricePerRoomTbx.Text = "";
            }
        }
        private void selectedApartmentEditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (apartmentsDgr.SelectedItem is Apartment selectedApartment)
                {
                    
                    if (string.IsNullOrWhiteSpace(selectedApartmentNameTbx.Text) ||
                        
                        !decimal.TryParse(selectedApartmentPricePerRoomTbx.Text, out decimal pricePerRoom))
                    {
                        MessageBox.Show("Please fill in all fields with valid data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    
                    var update = Builders<Apartment>.Update
                        .Set(a => a.ApartmentName, selectedApartmentNameTbx.Text)
                        .Set(a => a.PricePerRoom, pricePerRoom);

                    var filter = Builders<Apartment>.Filter.Eq(a => a.Id, selectedApartment.Id);
                    _db.Apartments.UpdateOne(filter, update);                    

                    
                    LoadApartments();

                    MessageBox.Show("Apartment updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Please select an apartment to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating apartment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void selectedApartmentDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (apartmentsDgr.SelectedItem is Apartment selectedApartment)
                {

                    var result = MessageBox.Show(
                    $"Are you sure you want to delete This apartment? This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                    if (result != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    var apartmentFilter = Builders<Apartment>.Filter.Eq(a => a.Id, selectedApartment.Id);
                    _db.Apartments.DeleteOne(apartmentFilter);

                    
                    var roomFilter = Builders<Room>.Filter.Eq(r => r.ApartmentNo, selectedApartment.ApartmentNo);
                    _db.Rooms.DeleteMany(roomFilter);

                    
                    LoadApartments();

                    
                    selectedApartmentNoTbx.Text = "";
                    selectedApartmentNameTbx.Text = "";
                    selectedApartmentPricePerRoomTbx.Text = "";

                    MessageBox.Show("Apartment and associated rooms deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Please select an apartment to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting apartment: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void addNewApartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Highest ApartmentNumber for the user default value passed 1
                var filter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(filter).ToList();
                var nextApartmentNumber = apartments.Any()
                    ? apartments.Max(a => int.Parse(a.ApartmentNo.Split('/')[2])) + 1
                    : 1;

                
                addApartmentNoTbx.Text = $"A/{LoggedInUser.UserID}/{nextApartmentNumber}";

                
                apartmentsOverview.Visibility = Visibility.Collapsed;
                addNewApartmentPnl.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error determining apartment number: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void addApartmentBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(addApartmentNameTbx.Text) ||
                    string.IsNullOrWhiteSpace(addApartmentAddressTbx.Text) ||
                    addApartmentRoomsTypeTbx.SelectedItem == null ||
                    !int.TryParse(addApartmentRoomsNoTbx.Text, out int noOfRooms) ||
                    !decimal.TryParse(addApartmentRoomsPriceTbx.Text, out decimal pricePerRoom))
                {
                    MessageBox.Show("Please fill in all fields with valid data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Extract ApartmentNumber
                string apartmentNo = addApartmentNoTbx.Text;
                string[] parts = apartmentNo.Split('/');
                if (parts.Length != 3 || !int.TryParse(parts[2], out int apartmentNumber))
                {
                    MessageBox.Show("Invalid apartment number format.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                
                var newApartment = new Apartment
                {
                    Id = Guid.NewGuid().ToString(),
                    ApartmentNo = apartmentNo,
                    ApartmentName = addApartmentNameTbx.Text,
                    Address = addApartmentAddressTbx.Text,
                    RoomsType = addApartmentRoomsTypeTbx.Text,
                    NoOfRooms = noOfRooms,
                    PricePerRoom = pricePerRoom,
                    UserId = LoggedInUser.Id
                };

                
                _db.Apartments.InsertOne(newApartment);

                // Create rooms
                for (int roomNumber = 1; roomNumber <= noOfRooms; roomNumber++)
                {
                    var newRoom = new Room
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoomNumber = $"R/{apartmentNumber}/{roomNumber}",
                        ApartmentNo = newApartment.ApartmentNo,
                        RoomType = newApartment.RoomsType,
                        PricePerRoom = newApartment.PricePerRoom,
                        RoomStatus = "Vacant",
                        RoomPaid = false
                    };
                    _db.Rooms.InsertOne(newRoom);
                }

                
                MessageBox.Show($"Apartment and {noOfRooms} room(s) added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                addNewApartmentPnl.Visibility = Visibility.Collapsed;
                apartmentsOverview.Visibility = Visibility.Visible;

                
                addApartmentNameTbx.Text = "";
                addApartmentAddressTbx.Text = "";
                addApartmentRoomsTypeTbx.SelectedIndex = -1;
                addApartmentRoomsNoTbx.Text = "";
                addApartmentRoomsPriceTbx.Text = "";

                LoadApartments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding apartment or rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backToAppartmentsLnk_Click(object sender, RoutedEventArgs e)
        {
            addNewApartmentPnl.Visibility = Visibility.Collapsed;
            apartmentsOverview.Visibility = Visibility.Visible;
        }
    }
}