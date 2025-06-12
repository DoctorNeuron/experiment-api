using System.Security.Cryptography;
using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExperimentAPI.Endpoint.Identity
{
    public class TokenEndpoint : Endpoint<TokenRequest, BaseResponse<TokenResponse>>
    {
        public override void Configure()
        {
            Post("identity/login");
            AllowAnonymous();
            AllowFormData(true);
        }

        public required ExperimentDBContext DB { get; set; }
        public required IOptions<TokenConfiguration> TokenConfig { get; set; }

        public override async Task<BaseResponse<TokenResponse>> ExecuteAsync(TokenRequest req, CancellationToken ct)
        {
            // Search by ID etc etc...
            var data = await DB.Set<MsUser>()
                .Include(x => x.RefreshToken)
                .Where(x => x.ID == req.ID)
                .FirstOrDefaultAsync(ct) ?? throw new Exceptions.UnauthorizedException();

            string refreshToken = RandomNumberGenerator.GetString(
                "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890",
                32
            );

            if (data.RefreshToken == null)
            {
                DB.Set<TrUserRefreshToken>()
                    .Add(new()
                    {
                        ID = Guid.NewGuid(),
                        ExpiredDate = DateTime.Now.ToUniversalTime().AddMinutes(TokenConfig.Value.RefreshTokenDurationMinutes),
                        UserID = req.ID,
                        RefreshToken = refreshToken
                    });
                await DB.SaveChangesAsync(ct);
            }
            else
            {
                data.RefreshToken.RefreshToken = refreshToken;
                data.RefreshToken.ExpiredDate = DateTime.Now.ToUniversalTime().AddMinutes(TokenConfig.Value.RefreshTokenDurationMinutes);
                DB.Update(data.RefreshToken);
                await DB.SaveChangesAsync(ct);
            }

            return new BaseResponse<TokenResponse>(
                new TokenResponse()
                {
                    Token = JwtBearer.CreateToken(o =>
                    {
                        o.ExpireAt = DateTime.Now.AddMinutes(TokenConfig.Value.AccessTokenDurationMinutes);
                        o.User.Claims.Add((Constant.Claims.ID, data.ID.ToString()));
                        o.User.Claims.Add((Constant.Claims.Username, data.Username));
                        o.User.Claims.Add((Constant.Claims.Email, data.Email));
                    }),
                    RefreshToken = refreshToken
                }
            );
        }
    }

    public class TokenRequest
    {
        public required Guid ID { get; set; }
    }

    public class TokenResponse
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
    }
}