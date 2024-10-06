using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class SubTasksController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnSubTaskRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateSubTaskRecord createSubTask){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        SubTask subTask = createSubTask.ToSubTask();
        await _database.SubTasks.InsertOneAsync(subTask);
        return CreatedAtRoute(new { id = subTask.Id }, ReturnSubTaskRecord.FromSubTask(subTask, newToken));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnSubTasksRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnSubTasksRecord.FromSubTasks(await _database.SubTasks.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnSubTaskRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnSubTaskRecord.FromSubTask(await _database.SubTasks.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("worker")]
    [Authorize]
    public async Task<ActionResult<ReturnSubTasksRecord>> GetByWorker([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnSubTasksRecord.FromSubTasks(await _database.SubTasks.Find(p => p.WorkerIds.Contains(id)).ToListAsync(), newToken));
    }
    [HttpGet("task")]
    [Authorize]
    public async Task<ActionResult<ReturnSubTasksRecord>> GetByTask([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Tasks task = await _database.Tasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (task == null) return ReturnSubTasksRecord.FromSubTasks([], newToken);
        return Ok(ReturnSubTasksRecord.FromSubTasks(await _database.SubTasks.Find(p => task.SubTaskIds.Contains(p.Id)).ToListAsync(), newToken));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateSubTaskRecord subtaskIn){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        SubTask subtask = await _database.SubTasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (subtask == null) return NotFound();
        SubTask replaced = SubTask.Replace(subtask, subtaskIn);
        await _database.SubTasks.ReplaceOneAsync(p => p.Id == id, replaced);
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
        SubTask subtask = await _database.SubTasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (subtask == null) return NotFound();
        await _database.SubTasks.DeleteOneAsync(p => p.Id == id);
        return newToken;
    }
}
