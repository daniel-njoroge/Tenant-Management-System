using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Tenant_Management_System.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string RoomNumber { get; set; }
        public string ApartmentNo { get; set; }
        public string RoomType { get; set; }
        public decimal PricePerRoom { get; set; }
        public string RoomStatus { get; set; }
        public bool RoomPaid { get; set; }
        public string TenantId { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? LastPaidDate { get; set; }
    }
}