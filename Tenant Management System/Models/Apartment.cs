using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tenant_Management_System.Models
{
    public class Apartment
    {
    public string Id { get; set; }
    public string ApartmentNo { get; set; }    
    public string ApartmentName { get; set; }
    public string Address { get; set; }
    public string RoomsType { get; set; }
    public int NoOfRooms { get; set; }
    public decimal PricePerRoom { get; set; }
    public string UserId { get; set; }
    }

}