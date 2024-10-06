using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class JobsController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnJobRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateJobRecord createJob){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Job job = createJob.ToJob();
        await _database.Jobs.InsertOneAsync(job);
        return CreatedAtRoute(new { id = job.Id }, ReturnJobRecord.FromJob(job, newToken));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnJobsRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnJobsRecord.FromJobs(await _database.Jobs.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnJobRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnJobRecord.FromJob(await _database.Jobs.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("task")]
    [Authorize]
    public async Task<ActionResult<ReturnJobRecord>> GetByTask([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnJobRecord.FromJob(await _database.Jobs.Find(p => p.TaskIds.Contains(id)).FirstOrDefaultAsync(), newToken));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<AuthToken>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateJobRecord jobIn){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Job job = await _database.Jobs.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (job == null) return NotFound();
        Job replaced = Job.Replace(job, jobIn);
        await _database.Jobs.ReplaceOneAsync(p => p.Id == id, replaced);
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
        Job job = await _database.Jobs.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (job == null) return NotFound();
        await _database.Jobs.DeleteOneAsync(p => p.Id == id);
        return newToken;
    }
}
