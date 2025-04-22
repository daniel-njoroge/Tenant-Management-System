using MongoDB.Driver;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class TenantsView : UserControl
    {
        private readonly User LoggedInUser;
        private readonly MongoDBConnection _db;

        public TenantsView(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _db = new MongoDBConnection();
            LoadTenants();
            PopulateApartmentComboBox();
        }

        private void LoadTenants()
        {
            try
            {
                var tenantFilter = Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id);
                var tenants = _db.Tenants.Find(tenantFilter).ToList();

                var tenantViewModels = tenants.Select(t =>
                {
                    var room = _db.Rooms.Find(r => r.TenantId == t.Id).FirstOrDefault();
                    return new Tenant
                    {
                        TenantNo = t.TenantNo,
                        Fullname = t.Fullname,
                        PhoneNumber = t.PhoneNumber,
                        Email = t.Email,
                        IdNumber = t.IdNumber,
                        RoomNumber = room != null ? room.RoomNumber : "Not Assigned"
                    };
                }).ToList();

                tenantsDgr.ItemsSource = tenantViewModels;
                statusLbl.Text = "";
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error loading tenants: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }

        private void PopulateApartmentComboBox()
        {
            try
            {
                var filter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(filter).ToList();
                selectedTenantApartmentNoCbx.ItemsSource = apartments.Select(a => a.ApartmentNo).ToList();
                selectedTenantApartmentNoCbx.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error loading apartments: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }

        private void PopulateRoomComboBox(string apartmentNo)
        {
            try
            {
                var filter = Builders<Room>.Filter.And(
                    Builders<Room>.Filter.Eq(r => r.ApartmentNo, apartmentNo),
                    Builders<Room>.Filter.Eq(r => r.RoomStatus, "Vacant"));
                var rooms = _db.Rooms.Find(filter).ToList();
                selectedTenantRoomNoCbx.ItemsSource = rooms.Select(r => r.RoomNumber).ToList();
                selectedTenantRoomNoCbx.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error loading rooms: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }

        private void addNewTenantBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var filter = Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id);
                var tenants = _db.Tenants.Find(filter).ToList();
                var nextTenantNumber = tenants.Any() ? tenants.Max(t => int.Parse(t.TenantNo.Split('/')[2])) + 1 : 1;
                addTenantNoTbx.Text = $"T/{LoggedInUser.UserID}/{nextTenantNumber}";

                tenantsOverview.Visibility = Visibility.Collapsed;
                addNewTenantPnl.Visibility = Visibility.Visible;
                addTenantStatusLbl.Text = "";
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error preparing to add tenant: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
            }
        }

        private void addTenantBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addTenantNameTbx.Text) ||
                    string.IsNullOrWhiteSpace(addTenantPhoneTbx.Text) ||
                    string.IsNullOrWhiteSpace(addTenantEmailTbx.Text) ||
                    string.IsNullOrWhiteSpace(addTenantIdNoTbx.Text))
                {
                    addTenantStatusLbl.Text = "All fields are required.";
                    addTenantStatusLbl.Foreground = Brushes.Red;
                    return;
                }

                if (!Regex.IsMatch(addTenantEmailTbx.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    addTenantStatusLbl.Text = "Invalid email format.";
                    addTenantStatusLbl.Foreground = Brushes.Red;
                    return;
                }

                if (!Regex.IsMatch(addTenantPhoneTbx.Text, @"^\+?\d{10,15}$"))
                {
                    addTenantStatusLbl.Text = "Invalid phone number (10-15 digits, optional +).";
                    addTenantStatusLbl.Foreground = Brushes.Red;
                    return;
                }

                if (_db.Tenants.Find(t => t.Email == addTenantEmailTbx.Text && t.UserId == LoggedInUser.Id).Any())
                {
                    addTenantStatusLbl.Text = "Email already registered for this user.";
                    addTenantStatusLbl.Foreground = Brushes.Red;
                    return;
                }

                var newTenant = new Tenant
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantNo = addTenantNoTbx.Text,
                    Fullname = addTenantNameTbx.Text,
                    PhoneNumber = addTenantPhoneTbx.Text,
                    Email = addTenantEmailTbx.Text,
                    IdNumber = addTenantIdNoTbx.Text,
                    UserId = LoggedInUser.Id
                };

                _db.Tenants.InsertOne(newTenant);

                addTenantNoTbx.Text = "";
                addTenantNameTbx.Text = "";
                addTenantPhoneTbx.Text = "";
                addTenantEmailTbx.Text = "";
                addTenantIdNoTbx.Text = "";
                addTenantStatusLbl.Text = "Tenant added successfully!";
                addTenantStatusLbl.Foreground = Brushes.Green;

                LoadTenants();
                addNewTenantPnl.Visibility = Visibility.Collapsed;
                tenantsOverview.Visibility = Visibility.Visible;

                PopulateApartmentComboBox();
            }
            catch (Exception ex)
            {
                addTenantStatusLbl.Text = $"Error adding tenant: {ex.Message}";
                addTenantStatusLbl.Foreground = Brushes.Red;
            }
        }

        private void assignRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedTenantNoTbx.Text))
                {
                    statusLbl.Text = "Please select a tenant.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                if (selectedTenantApartmentNoCbx.SelectedItem == null)
                {
                    statusLbl.Text = "Please select an apartment.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                if (selectedTenantRoomNoCbx.SelectedItem == null)
                {
                    statusLbl.Text = "Please select a room.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var tenantNo = selectedTenantNoTbx.Text;
                var apartmentNo = selectedTenantApartmentNoCbx.SelectedItem.ToString();
                var roomNumber = selectedTenantRoomNoCbx.SelectedItem.ToString();

                var tenantFilter = Builders<Tenant>.Filter.And(
                    Builders<Tenant>.Filter.Eq(t => t.TenantNo, tenantNo),
                    Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id));
                var tenant = _db.Tenants.Find(tenantFilter).FirstOrDefault();
                if (tenant == null)
                {
                    statusLbl.Text = "Invalid tenant selected.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var roomFilter = Builders<Room>.Filter.And(
                    Builders<Room>.Filter.Eq(r => r.RoomNumber, roomNumber),
                    Builders<Room>.Filter.Eq(r => r.ApartmentNo, apartmentNo),
                    Builders<Room>.Filter.Eq(r => r.RoomStatus, "Vacant"));
                var room = _db.Rooms.Find(roomFilter).FirstOrDefault();
                if (room == null)
                {
                    statusLbl.Text = "Selected room is not available.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                // Clear tenant's current room (if any)
                var currentRoomFilter = Builders<Room>.Filter.Eq(r => r.TenantId, tenant.Id);
                var currentRoom = _db.Rooms.Find(currentRoomFilter).FirstOrDefault();
                if (currentRoom != null)
                {
                    var clearUpdate = Builders<Room>.Update
                        .Set(r => r.RoomStatus, "Vacant")
                        .Set(r => r.TenantId, null);
                    _db.Rooms.UpdateOne(r => r.Id == currentRoom.Id, clearUpdate);
                }

                // Assign tenant to new room
                var update = Builders<Room>.Update
                    .Set(r => r.RoomStatus, "Occupied")
                    .Set(r => r.TenantId, tenant.Id);
                _db.Rooms.UpdateOne(r => r.Id == room.Id, update);

                statusLbl.Text = "Room assigned successfully!";
                statusLbl.Foreground = Brushes.Green;

                selectedTenantNoTbx.Text = "";
                selectedTenantApartmentNoCbx.SelectedIndex = -1;
                selectedTenantRoomNoCbx.SelectedIndex = -1;

                LoadTenants();
                PopulateApartmentComboBox(); // Refresh available rooms
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error assigning room: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
                MessageBox.Show($"Assignment error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void unassignRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedTenantNoTbx.Text))
                {
                    statusLbl.Text = "Please select a tenant.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var tenantNo = selectedTenantNoTbx.Text;
                var tenantFilter = Builders<Tenant>.Filter.And(
                    Builders<Tenant>.Filter.Eq(t => t.TenantNo, tenantNo),
                    Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id));
                var tenant = _db.Tenants.Find(tenantFilter).FirstOrDefault();
                if (tenant == null)
                {
                    statusLbl.Text = "Invalid tenant selected.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var roomFilter = Builders<Room>.Filter.Eq(r => r.TenantId, tenant.Id);
                var room = _db.Rooms.Find(roomFilter).FirstOrDefault();
                if (room == null)
                {
                    statusLbl.Text = "Tenant is not assigned to any room.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to unassign {tenant.Fullname} from room {room.RoomNumber}?",
                    "Confirm Unassignment",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                var update = Builders<Room>.Update
                    .Set(r => r.RoomStatus, "Vacant")
                    .Set(r => r.TenantId, null);
                _db.Rooms.UpdateOne(r => r.Id == room.Id, update);

                statusLbl.Text = "Room unassigned successfully!";
                statusLbl.Foreground = Brushes.Green;

                selectedTenantNoTbx.Text = "";
                LoadTenants();
                PopulateApartmentComboBox();
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error unassigning room: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
                MessageBox.Show($"Unassignment error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void deleteTenantBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedTenantNoTbx.Text))
                {
                    statusLbl.Text = "Please select a tenant.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var tenantNo = selectedTenantNoTbx.Text;
                var tenantFilter = Builders<Tenant>.Filter.And(
                    Builders<Tenant>.Filter.Eq(t => t.TenantNo, tenantNo),
                    Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id));
                var tenant = _db.Tenants.Find(tenantFilter).FirstOrDefault();
                if (tenant == null)
                {
                    statusLbl.Text = "Invalid tenant selected.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var result = MessageBox.Show(
                    $"Are you sure you want to delete tenant {tenant.Fullname} ({tenant.TenantNo})? This action cannot be undone.",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                // Unassign tenant from room (if assigned)
                var roomFilter = Builders<Room>.Filter.Eq(r => r.TenantId, tenant.Id);
                var room = _db.Rooms.Find(roomFilter).FirstOrDefault();
                if (room != null)
                {
                    var update = Builders<Room>.Update
                        .Set(r => r.RoomStatus, "Vacant")
                        .Set(r => r.TenantId, null);
                    _db.Rooms.UpdateOne(r => r.Id == room.Id, update);
                }

                // Delete tenant
                _db.Tenants.DeleteOne(t => t.Id == tenant.Id);

                statusLbl.Text = "Tenant deleted successfully!";
                statusLbl.Foreground = Brushes.Green;

                selectedTenantNoTbx.Text = "";
                LoadTenants();
                PopulateApartmentComboBox();
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error deleting tenant: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
                MessageBox.Show($"Deletion error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void tenantsDgr_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tenantsDgr.SelectedItem is Tenant selectedTenant)
            {
                selectedTenantNoTbx.Text = selectedTenant.TenantNo;
                statusLbl.Text = "";
            }
            else
            {
                selectedTenantNoTbx.Text = "";
            }

            PopulateApartmentComboBox();
        }

        private void selectedTenantApartmentNoCbx_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedTenantApartmentNoCbx.SelectedItem != null)
            {
                var apartmentNo = selectedTenantApartmentNoCbx.SelectedItem.ToString();
                PopulateRoomComboBox(apartmentNo);
            }
            else
            {
                selectedTenantRoomNoCbx.ItemsSource = null;
                selectedTenantRoomNoCbx.SelectedIndex = -1;
            }
        }


        private void markPaidBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(selectedTenantNoTbx.Text))
                {
                    statusLbl.Text = "Please select a tenant.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var tenantNo = selectedTenantNoTbx.Text;
                var tenantFilter = Builders<Tenant>.Filter.And(
                    Builders<Tenant>.Filter.Eq(t => t.TenantNo, tenantNo),
                    Builders<Tenant>.Filter.Eq(t => t.UserId, LoggedInUser.Id));
                var tenant = _db.Tenants.Find(tenantFilter).FirstOrDefault();
                if (tenant == null)
                {
                    statusLbl.Text = "Invalid tenant selected.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var roomFilter = Builders<Room>.Filter.Eq(r => r.TenantId, tenant.Id);
                var room = _db.Rooms.Find(roomFilter).FirstOrDefault();
                if (room == null)
                {
                    statusLbl.Text = "Tenant is not assigned to any room.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                // Check if a payment exists for the current month
                var currentMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                var nextMonthStart = currentMonthStart.AddMonths(1);
                var paymentFilter = Builders<Payment>.Filter.And(
                    Builders<Payment>.Filter.Eq(p => p.RoomId, room.Id),
                    Builders<Payment>.Filter.Eq(p => p.TenantId, tenant.Id),
                    Builders<Payment>.Filter.Gte(p => p.PaymentDate, currentMonthStart),
                    Builders<Payment>.Filter.Lt(p => p.PaymentDate, nextMonthStart));
                var existingPayment = _db.Payments.Find(paymentFilter).FirstOrDefault();
                if (existingPayment != null || room.RoomPaid)
                {
                    statusLbl.Text = $"This room is already paid for {currentMonthStart:MMMM yyyy}.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                var result = MessageBox.Show(
                    $"Mark {tenant.Fullname} as paid for room {room.RoomNumber} (Amount: {room.PricePerRoom})?",
                    "Confirm Payment",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                // Update room to paid and set LastPaidDate
                var roomUpdate = Builders<Room>.Update
                    .Set(r => r.RoomPaid, true)
                    .Set(r => r.LastPaidDate, DateTime.UtcNow);
                var updateResult = _db.Rooms.UpdateOne(r => r.Id == room.Id, roomUpdate);
                if (updateResult.ModifiedCount == 0)
                {
                    statusLbl.Text = "Failed to update room payment status.";
                    statusLbl.Foreground = Brushes.Red;
                    return;
                }

                
                var payment = new Payment
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = tenant.Id,
                    RoomId = room.Id,
                    ApartmentNo = room.ApartmentNo,
                    Amount = room.PricePerRoom,
                    PaymentDate = DateTime.UtcNow,
                    UserId = LoggedInUser.Id
                };
                _db.Payments.InsertOne(payment);

                statusLbl.Text = "Tenant marked as paid successfully!";
                statusLbl.Foreground = Brushes.Green;

                LoadTenants();
            }
            catch (Exception ex)
            {
                statusLbl.Text = $"Error marking tenant as paid: {ex.Message}";
                statusLbl.Foreground = Brushes.Red;
                MessageBox.Show($"Payment error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void backToTenantsLnk_Click(object sender, RoutedEventArgs e)
        {
            addNewTenantPnl.Visibility = Visibility.Collapsed;
            tenantsOverview.Visibility = Visibility.Visible;
            addTenantStatusLbl.Text = "";
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadTenants();
            PopulateApartmentComboBox();
        }
    }
}