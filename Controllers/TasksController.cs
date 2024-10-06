using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnTaskRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateTaskRecord createTask){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Tasks task = createTask.ToTask();
        await _database.Tasks.InsertOneAsync(task);
        return CreatedAtRoute(new { id = task.Id }, ReturnTaskRecord.FromTask(task, newToken));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnTasksRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnTasksRecord.FromTasks(await _database.Tasks.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnTaskRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnTaskRecord.FromTask(await _database.Tasks.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("division")]
    [Authorize]
    public async Task<ActionResult<ReturnTasksRecord>> GetByDivision([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnTasksRecord.FromTasks(await _database.Tasks.Find(p => p.DivisionId == id).ToListAsync(), newToken));
    }
    [HttpGet("subtask")]
    [Authorize]
    public async Task<ActionResult<ReturnTaskRecord>> GetBySubTask([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnTaskRecord.FromTask(await _database.Tasks.Find(p => p.SubTaskIds.Contains(id)).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("job")]
    [Authorize]
    public async Task<ActionResult<ReturnTasksRecord>> GetByJob([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Job job = await _database.Jobs.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (job == null) return ReturnTasksRecord.FromTasks([], newToken);
        return Ok(ReturnTasksRecord.FromTasks(await _database.Tasks.Find(p => job.TaskIds.Contains(p.Id)).ToListAsync(), newToken));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateTaskRecord taskIn){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Tasks task = await _database.Tasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (task == null) return NotFound();
        Tasks replaced = Tasks.Replace(task, taskIn);
        await _database.Tasks.ReplaceOneAsync(p => p.Id == id, replaced);
        return newToken;
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Tasks task = await _database.Tasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (task == null) return NotFound();
        await _database.Tasks.DeleteOneAsync(p => p.Id == id);
        return newToken;
    }
}
