using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Apartment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Apartment Number")]
    public string ApartmentNo { get; set; }

    [BsonElement("Apartment Name")]
    public string ApartmentName { get; set; }

    [BsonElement("Adress")]
    public string Address { get; set; }

    [BsonElement("Rooms Type")]
    public string RoomsType { get; set; }

    [BsonElement("Number of Rooms")]
    public string NoOfRooms { get; set; }

    [BsonElement("Room Price")]
    public string RoomPrice { get; set; }
}