using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Tenant
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Tenant Number")]
    public string TenantNo { get; set; }

    [BsonElement("Full Name")]
    public string Fullname { get; set; }

    [BsonElement("Phone Number")]
    public string PhoneNumber { get; set; }

    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("ID Number")]
    public string IdNumber { get; set; }
}