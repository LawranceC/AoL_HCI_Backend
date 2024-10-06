using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Division{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
}

public record ReturnDivisionRecord{
    [BsonElement("Division")]
    public Division Division { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnDivisionRecord FromDivision(Division division, AuthToken token){
        return new ReturnDivisionRecord{
            Division = division,
            Token = token
        };
    }
}

public record ReturnDivisionsRecord{
    [BsonElement("Divisions")]
    public List<Division> Divisions { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnDivisionsRecord FromDivisions(List<Division> divisions, AuthToken token){
        return new ReturnDivisionsRecord{
            Divisions = divisions,
            Token = token
        };
    }
}

public record CreateDivisionRecord{
    [BsonElement("Name")]
    public string Name { get; set; } = null!;

    public Division ToDivision(){
        return new Division{
            Name = Name
        };
    }
}