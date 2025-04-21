using MongoDB.Driver;
using System.Configuration;
using Tenant_Management_System.Models;

public class MongoDBConnection
{
    private readonly IMongoDatabase _database;

    public MongoDBConnection()
    {
        string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("TMS");
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");
    public IMongoCollection<Apartment> Apartments =>
        _database.GetCollection<Apartment>("Apartments");
    public IMongoCollection<Room> Rooms =>
        _database.GetCollection<Room>("Rooms");
    public IMongoCollection<Tenant> Tenants =>
        _database.GetCollection<Tenant>("Tenants");
}
