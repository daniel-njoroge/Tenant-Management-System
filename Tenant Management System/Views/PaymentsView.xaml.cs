using MongoDB.Driver;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tenant_Management_System.Models;

namespace Tenant_Management_System.Views
{
    public partial class PaymentsView : UserControl
    {
        private readonly User LoggedInUser;
        private readonly MongoDBConnection _db;

        public PaymentsView(User user)
        {
            InitializeComponent();
            LoggedInUser = user;
            _db = new MongoDBConnection();
            ResetUnpaidRooms();
            LoadPayments();
        }

        private void ResetUnpaidRooms()
        {
            try
            {
                var apartmentFilter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(apartmentFilter).ToList();
                var apartmentNos = apartments.Select(a => a.ApartmentNo).ToList();

                var roomFilter = Builders<Room>.Filter.And(
                    Builders<Room>.Filter.In(r => r.ApartmentNo, apartmentNos),
                    Builders<Room>.Filter.Eq(r => r.RoomPaid, true),
                    Builders<Room>.Filter.Ne(r => r.LastPaidDate, null));
                var rooms = _db.Rooms.Find(roomFilter).ToList();

                foreach (var room in rooms)
                {
                    if (room.LastPaidDate.HasValue && room.LastPaidDate.Value.AddMonths(1) < DateTime.UtcNow)
                    {
                        var update = Builders<Room>.Update.Set(r => r.RoomPaid, false);
                        _db.Rooms.UpdateOne(r => r.Id == room.Id, update);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting unpaid rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPayments()
        {
            try
            {
                // Load payments
                var paymentFilter = Builders<Payment>.Filter.Eq(p => p.UserId, LoggedInUser.Id);
                var payments = _db.Payments.Find(paymentFilter).ToList();

                // Prepare view model for DataGrid
                var paymentViewModels = payments.Select(p =>
                {
                    var tenant = _db.Tenants.Find(t => t.Id == p.TenantId).FirstOrDefault();
                    var room = _db.Rooms.Find(r => r.Id == p.RoomId).FirstOrDefault();
                    return new
                    {
                        TenantName = tenant?.Fullname ?? "Unknown",
                        RoomNumber = room?.RoomNumber ?? "Unknown",
                        p.ApartmentNo,
                        p.Amount,
                        p.PaymentDate
                    };
                }).ToList();

                paymentsDgr.ItemsSource = paymentViewModels;

                // Calculate summaries
                decimal totalPaid = payments.Sum(p => p.Amount);
                totalPaidLbl.Text = totalPaid.ToString("F2");

                // Calculate pending payments (rooms that are occupied but not paid)
                var apartmentFilter = Builders<Apartment>.Filter.Eq(a => a.UserId, LoggedInUser.Id);
                var apartments = _db.Apartments.Find(apartmentFilter).ToList();
                var apartmentNos = apartments.Select(a => a.ApartmentNo).ToList();

                var occupiedRoomsFilter = Builders<Room>.Filter.And(
                    Builders<Room>.Filter.In(r => r.ApartmentNo, apartmentNos),
                    Builders<Room>.Filter.Eq(r => r.RoomStatus, "Occupied"),
                    Builders<Room>.Filter.Eq(r => r.RoomPaid, false));
                var occupiedRooms = _db.Rooms.Find(occupiedRoomsFilter).ToList();
                decimal pendingPayments = occupiedRooms.Sum(r => r.PricePerRoom);
                pendingPaymentsLbl.Text = pendingPayments.ToString("F2");

                // Calculate this month's payments
                var thisMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
                decimal thisMonthPayments = payments
                    .Where(p => p.PaymentDate >= thisMonth)
                    .Sum(p => p.Amount);
                thisMonthLbl.Text = thisMonthPayments.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payments: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            ResetUnpaidRooms();
            LoadPayments();
        }
    }
}