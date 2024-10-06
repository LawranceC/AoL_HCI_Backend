using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DivisionsController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReturnDivisionRecord>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateDivisionRecord createDivision){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Division division = createDivision.ToDivision();
        await _database.Divisions.InsertOneAsync(division);
        return CreatedAtRoute(new { id = division.Id }, ReturnDivisionRecord.FromDivision(division, newToken));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnDivisionsRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnDivisionsRecord.FromDivisions(await _database.Divisions.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnDivisionRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnDivisionRecord.FromDivision(await _database.Divisions.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("task")]
    [Authorize]
    public async Task<ActionResult<ReturnDivisionRecord>> GetByTask([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Tasks task = await _database.Tasks.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (task == null) return ReturnDivisionRecord.FromDivision(null, newToken);
        return Ok(ReturnDivisionRecord.FromDivision(await _database.Divisions.Find(p => p.Id == task.DivisionId).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("worker")]
    [Authorize]
    public async Task<ActionResult<ReturnDivisionRecord>> GetByWorker([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Worker worker = await _database.Workers.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (worker == null) return ReturnDivisionRecord.FromDivision(null, newToken);
        return Ok(ReturnDivisionRecord.FromDivision(await _database.Divisions.Find(p => p.Id == worker.DivisionId).FirstOrDefaultAsync(), newToken));
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
        Division division = await _database.Divisions.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (division == null) return NotFound();
        await _database.Divisions.DeleteOneAsync(p => p.Id == id);
        return newToken;
    }
}
