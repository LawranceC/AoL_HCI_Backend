using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Worker{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("IdentityId")]
    public string IdentityId { get; set; } = null!;
    [BsonElement("RoleId")]
    public string RoleId { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;
    
    public static Worker Replace(Worker worker, UpdateWorkerRecord workerIn){
        return new Worker{
            Id = worker.Id,
            Name = workerIn.Name is not null? workerIn.Name : worker.Name,
            RoleId = workerIn.RoleId is not null? workerIn.RoleId : worker.RoleId,
            DivisionId = workerIn.DivisionId is not null? workerIn.DivisionId : worker.DivisionId,
            IdentityId = worker.IdentityId
        };
    }
}

public record ReturnWorkerRecord{
    [BsonElement("Worker")]
    public Worker Worker { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnWorkerRecord FromWorker(Worker worker, AuthToken token){
        return new ReturnWorkerRecord{
            Worker = worker,
            Token = token
        };
    }
}

public record ReturnWorkersRecord{
    [BsonElement("Workers")]
    public List<Worker> Workers { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnWorkersRecord FromWorkers(List<Worker> workers, AuthToken token){
        return new ReturnWorkersRecord{
            Workers = workers,
            Token = token
        };
    }
}

public record RegisterWorkerRecord{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Email")]
    public string Email { get; set; } = null!;
    [BsonElement("Password")]
    public string Password { get; set; } = null!;
    [BsonElement("RoleId")]
    public string RoleId { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;

    public CreateWorkerRecord ToCreateWorkerRecord(){
        return new CreateWorkerRecord{
            Name = Name,
            RoleId = RoleId,
            DivisionId = DivisionId
        };
    }
}

public record CreateWorkerRecord{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("RoleId")]
    public string RoleId { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;

    public Worker ToWorker(){
        return new Worker{
            Name = Name,
            RoleId = RoleId,
            DivisionId = DivisionId
        };
    }
}

public record LoginRecord{
    [BsonElement("Email")]
    public string Email { get; set; } = null!;
    [BsonElement("Password")]
    public string Password { get; set; } = null!;
}

public record UpdateWorkerRecord{
    [BsonElement("Name")]
    public string? Name { get; set; } = null!;
    [BsonElement("RoleId")]
    public string? RoleId { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string? DivisionId { get; set; } = null!;
    [BsonElement("Email")]
    public string? Email { get; set; } = null!;
    [BsonElement("Password")]
    public string? Password { get; set; } = null!;

    public Worker ToWorker(){
        return new Worker{
            Name = Name,
            RoleId = RoleId,
            DivisionId = DivisionId
        };
    }
}