using MongoDB.Driver;
using System.Configuration;

public class MongoDBConnection
{
    private readonly IMongoDatabase _database;

    public MongoDBConnection()
    {
        string connectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("AuthDB");
    }

    public IMongoCollection<User> Users =>
        _database.GetCollection<User>("Users");
}
