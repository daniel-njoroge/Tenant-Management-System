using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tenant_Management_System.Models
{
    public class Tenant
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public string TenantNo { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string IdNumber { get; set; }
    }
}