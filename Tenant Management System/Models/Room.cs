using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Room
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("Room Number")]
    public string RoomNumber { get; set; }

    [BsonElement("Apartment Number")]
    public string ApartmentNo { get; set; }

    [BsonElement("Room Type")]
    public string RoomType { get; set; }

    [BsonElement("Room Price")]
    public string RoomPrice { get; set; }

    [BsonElement("Room Status")]
    public string RoomStatus { get; set; }

    [BsonElement("RoomPaid")]
    public string RoomPaid { get; set; }
}