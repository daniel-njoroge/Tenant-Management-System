using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tenant_Management_System.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string RoomNumber { get; set; } // e.g., R/3/4
        public string ApartmentNo { get; set; } // e.g., A/1/3
        public string RoomType { get; set; } // e.g., Single
        public decimal PricePerRoom { get; set; } // e.g., 1000
        public string RoomStatus { get; set; } // e.g., Vacant
        public bool RoomPaid { get; set; } // e.g., false
    }
}