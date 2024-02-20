using AozoraSharp.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Timers;

namespace AozoraSharp;

public class SessionKeeper
{
    private readonly JwtSecurityTokenHandler tokenHandler = new();
    private readonly TokenValidationParameters validationParameters = new()
    {
        ValidateSignatureLast = false,
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        ValidateTokenReplay = false,
        ValidateWithLKG = false,
        ValidateIssuerSigningKey = false,
        SignatureValidator = (token, _) => new JwtSecurityToken(token),
    };
    private readonly ILogger logger = LogManager.Instance.GetLogger(nameof(SessionKeeper));

    public void TokenRenewed(AozoraClient client)
    {
        var accessToken = client.AccessToken;
        tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
        var expiresAtUtc = validatedToken.ValidTo.ToUniversalTime();
        logger.Debug($"Access token expires at {expiresAtUtc:o}");
        var span = expiresAtUtc - DateTime.UtcNow;
        // 1分前行動
        // アクセストークンの有効期限は2時間(2024-02-14時点)
        span -= TimeSpan.FromMinutes(1);
        var timer = new Timer(span.TotalMilliseconds);
        timer.Elapsed += async (_, _) =>
        {
            await client.RefreshSessionAsync();
            timer.Dispose();
        };
        timer.Start();
    }
}
