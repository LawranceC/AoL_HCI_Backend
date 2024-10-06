using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Role
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
}

public record ReturnRoleRecord{
    [BsonElement("Role")]
    public Role Role { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnRoleRecord FromRole(Role role, AuthToken token){
        return new ReturnRoleRecord{
            Role = role,
            Token = token
        };
    }
}

public record ReturnRolesRecord{
    [BsonElement("Roles")]
    public List<Role> Roles { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnRolesRecord FromRoles(List<Role> roles, AuthToken token){
        return new ReturnRolesRecord{
            Roles = roles,
            Token = token
        };
    }
}