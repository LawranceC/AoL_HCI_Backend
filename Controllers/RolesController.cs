using System;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class RolesController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
{
    private readonly MongoDBServices _database = database;
    private readonly IAuthenticationServices _authentication = authentication;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ReturnRolesRecord>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnRolesRecord.FromRoles(await _database.Roles.Find(_ => true).ToListAsync(), newToken));
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<ActionResult<ReturnRoleRecord>> GetById([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        return Ok(ReturnRoleRecord.FromRole(await _database.Roles.Find(p => p.Id == id).FirstOrDefaultAsync(), newToken));
    }
    [HttpGet("worker")]
    [Authorize]
    public async Task<ActionResult<ReturnRoleRecord>> GetByWorker([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
        var tokenArr = token.Split(" ");
        var newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
        if(JwtValidator.TokenIsExpired(tokenArr[1])){
            newToken = await _authentication.RefreshToken(refreshToken);
            if(newToken.IdToken is null) return Unauthorized("Token Expired");
        }
        Worker worker = await _database.Workers.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (worker == null) return ReturnRoleRecord.FromRole(null, newToken);
        return Ok(ReturnRoleRecord.FromRole(await _database.Roles.Find(p => p.Id == worker.RoleId).FirstOrDefaultAsync(), newToken));
    }
}
