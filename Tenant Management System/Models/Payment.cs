using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Tenant_Management_System.Models
{
    public class Payment
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string RoomId { get; set; }
        public string ApartmentNo { get; set; }
        public decimal Amount { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime PaymentDate { get; set; }
        public string UserId { get; set; }
    }
}