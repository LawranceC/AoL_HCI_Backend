using System;
using FirebaseAdmin.Auth;
using MongoDB.Bson;

namespace AoL_HCI_Backend.Services;

public interface IAuthenticationServices{
    public Task<string?> Register(string email, string password);
    public Task<AuthToken?> Login(string email, string password);
    public Task<AuthToken?> RefreshToken(string refreshToken);
}

internal sealed class AuthenticationServices(IJwtProviderServices jwtProvider, IJwtRefreshTokenServices jwtRefreshToken) : IAuthenticationServices
{
    private readonly IJwtProviderServices _jwtProvider = jwtProvider;
    private readonly IJwtRefreshTokenServices _jwtRefreshToken = jwtRefreshToken;
    public async Task<string?> Register(string email, string password){
        UserRecord? user = null;
        try{
            user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
        } catch (Exception) {};
        if(user != null) return null;
        var newUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs{
            Email = email,
            Password = password
        });
        return newUser.Uid;
    }

    public async Task<AuthToken?> Login(string email, string password){
        var token = await _jwtProvider.GenerateToken(email, password);
        return token;
    }

    public async Task<AuthToken?> RefreshToken(string refreshToken){
        var token = await _jwtRefreshToken.RefreshToken(refreshToken);
        return token;
    }

    public static async void UpdateEmail(string identityId, string email){
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs{
            Uid = identityId,
            Email = email
        });
    }

    public static async void UpdatePassword(string identityId, string password){
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs{
            Uid = identityId,
            Password = password
        });
    }

    public static async void Unregister(string identityId){
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(identityId);
    }
}
