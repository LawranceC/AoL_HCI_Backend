using System;
using AoL_HCI_Backend.Models;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Services;

public class MongoDBServices
{
    private readonly IMongoDatabase _database;

    public MongoDBServices(string ConnectionString, string DatabaseName){
        var client = new MongoClient(ConnectionString);
        _database = client.GetDatabase(DatabaseName);
    }

    public IMongoCollection<Division> Divisions => _database.GetCollection<Division>("Divisions");
    public IMongoCollection<Role> Roles => _database.GetCollection<Role>("Roles");
    public IMongoCollection<Worker> Workers => _database.GetCollection<Worker>("Workers");
    public IMongoCollection<SubTask> SubTasks => _database.GetCollection<SubTask>("SubTasks");
    public IMongoCollection<Tasks> Tasks => _database.GetCollection<Tasks>("Tasks");
    public IMongoCollection<Job> Jobs => _database.GetCollection<Job>("Jobs");
}
