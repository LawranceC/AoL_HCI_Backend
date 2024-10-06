using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class SubTask
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("WorkerIds")]
    public string[] WorkerIds { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; } = 0;
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;

    public static SubTask Replace(SubTask subTask, UpdateSubTaskRecord subTaskIn){
        return new SubTask{
            Id = subTask.Id,
            Title = subTaskIn.Title is not null? subTaskIn.Title : subTask.Title,
            Description = subTaskIn.Description is not null? subTaskIn.Description : subTask.Description,
            WorkerIds = subTaskIn.WorkerIds is not null? subTaskIn.WorkerIds : subTask.WorkerIds,
            Status = (int)(subTaskIn.Status is not null? subTaskIn.Status : subTask.Status),
            IssueDate = subTask.IssueDate,
            Deadline = subTaskIn.Deadline is not null? subTaskIn.Deadline : subTask.Deadline
        };
    }
}

public record ReturnSubTaskRecord{
    [BsonElement("SubTask")]
    public SubTask SubTask { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnSubTaskRecord FromSubTask(SubTask subTask, AuthToken token){
        return new ReturnSubTaskRecord{
            SubTask = subTask,
            Token = token
        };
    }
}

public record ReturnSubTasksRecord{
    [BsonElement("SubTasks")]
    public List<SubTask> SubTasks { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnSubTasksRecord FromSubTasks(List<SubTask> subTasks, AuthToken token){
        return new ReturnSubTasksRecord{
            SubTasks = subTasks,
            Token = token
        };
    }
}

public record CreateSubTaskRecord{
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("WorkerIds")]
    public string[] WorkerIds { get; set; } = null!;
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;
    public SubTask ToSubTask(){
        return new SubTask{
            Title = Title,
            Description = Description,
            WorkerIds = WorkerIds,
            Deadline = Deadline
        };
    }
}

public record UpdateSubTaskRecord{
    [BsonElement("Title")]
    public string? Title { get; set; } = null!;
    [BsonElement("Description")]
    public string? Description { get; set; } = null!;
    [BsonElement("WorkerIds")]
    public string[]? WorkerIds { get; set; } = null!;
    [BsonElement("Status")]
    public int? Status { get; set; } = null!;
    [BsonElement("Deadline")]
    public string? Deadline { get; set; } = null!;
}