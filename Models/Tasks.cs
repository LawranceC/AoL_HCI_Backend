using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Tasks
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;
    [BsonElement("SubTaskIds")]
    public string[] SubTaskIds { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; } = 0;
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;

    public static Tasks Replace(Tasks task, UpdateTaskRecord taskIn){
        return new Tasks{
            Id = task.Id,
            Title = taskIn.Title is not null? taskIn.Title : task.Title,
            DivisionId = taskIn.DivisionId is not null? taskIn.DivisionId : task.DivisionId,
            SubTaskIds = taskIn.SubTaskIds is not null? taskIn.SubTaskIds : task.SubTaskIds,
            Status = (int)(taskIn.Status is not null? taskIn.Status : task.Status),
            IssueDate = task.IssueDate,
            Deadline = taskIn.Deadline is not null? taskIn.Deadline : task.Deadline
        };
    }
}

public record ReturnTaskRecord{
    [BsonElement("Task")]
    public Tasks Task { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnTaskRecord FromTask(Tasks task, AuthToken token){
        return new ReturnTaskRecord{
            Task = task,
            Token = token
        };
    }
}

public record ReturnTasksRecord{
    [BsonElement("Tasks")]
    public List<Tasks> Tasks { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnTasksRecord FromTasks(List<Tasks> tasks, AuthToken token){
        return new ReturnTasksRecord{
            Tasks = tasks,
            Token = token
        };
    }
}

public record CreateTaskRecord{
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string DivisionId { get; set; } = null!;
    [BsonElement("SubTaskIds")]
    public string[] SubTaskIds { get; set; } = null!;
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;
    public Tasks ToTask(){
        return new Tasks{
            Title = Title,
            DivisionId = DivisionId,
            SubTaskIds = SubTaskIds,
            Deadline = Deadline
        };
    }
}

public record UpdateTaskRecord{
    [BsonElement("Title")]
    public string? Title { get; set; } = null!;
    [BsonElement("DivisionId")]
    public string? DivisionId { get; set; } = null!;
    [BsonElement("SubTaskIds")]
    public string[]? SubTaskIds { get; set; } = null!;
    [BsonElement("Status")]
    public int? Status { get; set; } = null!;
    [BsonElement("Deadline")]
    public string? Deadline { get; set; } = null!;
}