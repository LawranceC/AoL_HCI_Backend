using System;
using AoL_HCI_Backend.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AoL_HCI_Backend.Models;

public class Job
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  = null!;
    [BsonElement("Issuer")]
    public string Issuer { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("TaskIds")]
    public string[] TaskIds { get; set; } = null!;
    [BsonElement("IssueDate")]
    public string IssueDate { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;

    public static Job Replace(Job job, UpdateJobRecord jobIn){
        return new Job{
            Id = job.Id,
            Issuer = jobIn.Issuer is not null? jobIn.Issuer : job.Issuer,
            Title = jobIn.Title is not null? jobIn.Title : job.Title,
            TaskIds = jobIn.TaskIds is not null? jobIn.TaskIds : job.TaskIds,
            IssueDate = job.IssueDate,
            Deadline = jobIn.Deadline is not null? jobIn.Deadline : job.Deadline
        };
    }
}

public record ReturnJobRecord{
    [BsonElement("Job")]
    public Job Job { get; set; } = null!;
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;

    public static ReturnJobRecord FromJob(Job job, AuthToken token){
        return new ReturnJobRecord{
            Job = job,
            Token = token
        };
    }
}

public record ReturnJobsRecord{
    [BsonElement("Jobs")]
    public List<Job> Jobs { get; set; } = [];
    [BsonElement("Token")]
    public AuthToken Token { get; set; } = null!;
    
    public static ReturnJobsRecord FromJobs(List<Job> jobs, AuthToken token){
        return new ReturnJobsRecord{
            Jobs = jobs,
            Token = token
        };
    }
}

public record CreateJobRecord{
    [BsonElement("Issuer")]
    public string Issuer { get; set; } = null!;
    [BsonElement("Title")]
    public string Title { get; set; } = null!;
    [BsonElement("TaskIds")]
    public string[] TaskIds { get; set; } = null!;
    [BsonElement("Deadline")]
    public string Deadline { get; set; } = null!;

    public Job ToJob(){
        return new Job{
            Issuer = Issuer,
            Title = Title,
            TaskIds = TaskIds,
            Deadline = Deadline
        };
    }
}

public record UpdateJobRecord{
    [BsonElement("Issuer")]
    public string? Issuer { get; set; } = null!;
    [BsonElement("Title")]
    public string? Title { get; set; } = null!;
    [BsonElement("TaskIds")]
    public string[]? TaskIds { get; set; } = null!;
    [BsonElement("Deadline")]
    public string? Deadline { get; set; } = null!;
}