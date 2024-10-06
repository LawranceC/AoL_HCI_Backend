using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkersController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnWorkerRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, RegisterWorkerRecord registerWorker){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Worker worker = registerWorker.ToCreateWorkerRecord().ToWorker();
        var IdentityId = _authentication.Register(registerWorker.Email, registerWorker.Password).Result;
        if(IdentityId == null) return BadRequest("Email Address Already Exist");
        worker.IdentityId = IdentityId;
        await _database.Workers.InsertOneAsync(worker);
        return CreatedAtRoute(new { id = worker.Id }, ReturnWorkerRecord.FromWorker(worker, newToken));
    }
    [HttpPost("login")]
    public async Task<ActionResult<AuthToken>> Login(LoginRecord credential){
        var token = await _authentication.Login(credential.Email, credential.Password);
        if(token.IdToken is null) return BadRequest("Invalid Credential");
        return token;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnWorkersRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnWorkersRecord.FromWorkers(await _database.Workers.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnWorkerRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnWorkerRecord.FromWorker(await _database.Workers.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("division")]
    [Authorize]
    public async Task<ActionResult<ReturnWorkersRecord>> GetByDvision([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnWorkersRecord.FromWorkers(await _database.Workers.Find(p => p.DivisionId == id).ToListAsync(), newToken));
    }
    [HttpGet("role")]
    [Authorize]
    public async Task<ActionResult<ReturnWorkersRecord>> GetByRole([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnWorkersRecord.FromWorkers(await _database.Workers.Find(p => p.RoleId == id).ToListAsync(), newToken));
    }
    [HttpGet("division-role")]
    [Authorize]
    public async Task<ActionResult<ReturnWorkersRecord>> GetByDivisionRole([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string divisionId, string roleId){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnWorkersRecord.FromWorkers(await _database.Workers.Find(p => p.DivisionId == divisionId && p.RoleId == roleId).ToListAsync(), newToken));
    }
    [HttpGet("subtask")]
    [Authorize]
    public async Task<ActionResult<ReturnWorkersRecord>> GetBySubTask([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        SubTask subtask = await _database.SubTasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (subtask == null) return ReturnWorkersRecord.FromWorkers([], newToken);
        return Ok(ReturnWorkersRecord.FromWorkers(await _database.Workers.Find(p => subtask.WorkerIds.Contains(p.Id)).ToListAsync(), newToken));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateWorkerRecord workerIn){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Worker worker = await _database.Workers.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (worker == null) return NotFound();
        Worker replaced = Worker.Replace(worker, workerIn);
        await _database.Workers.ReplaceOneAsync(p => p.Id == id, replaced);
        if(workerIn.Email is not null) AuthenticationServices.UpdateEmail(worker.IdentityId, workerIn.Email);
        if(workerIn.Password is not null) AuthenticationServices.UpdatePassword(worker.IdentityId, workerIn.Password);
        return newToken;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<AuthToken>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Worker worker = await _database.Workers.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (worker == null) return NotFound();
        AuthenticationServices.Unregister(worker.IdentityId);
        await _database.Workers.DeleteOneAsync(p => p.Id == id);
        return newToken;
    }    
}
