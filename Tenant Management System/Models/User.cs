using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Full Name")]
    public string Fullname { get; set; }

    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("Password")]
    public string Password { get; set; }
}