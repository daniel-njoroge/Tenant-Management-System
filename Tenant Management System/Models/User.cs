using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tenant_Management_System.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }
        public int UserID { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}