using ExperimentAPI.DTO;
using ExperimentAPI.Entity;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ExperimentAPI.Endpoint.Identity
{
    public class RefreshEndpoint : Endpoint<RefreshRequest, BaseResponse<RefreshResponse>>
    {
        public override void Configure()
        {
            Post("identity/refresh");
            Description(x => x.Accepts<RefreshRequest>());
            AllowAnonymous();
        }

        public required ExperimentDBContext DB { get; set; }
        public required IOptions<TokenConfiguration> TokenConfig { get; set; }

        public override async Task<BaseResponse<RefreshResponse>> ExecuteAsync(RefreshRequest req, CancellationToken ct)
        {
            // Search by ID etc etc...
            var data = await DB.Set<MsUser>()
                .Include(x => x.RefreshToken)
                .Where(x =>
                    x.RefreshToken != null &&
                    x.RefreshToken.ExpiredDate > DateTime.UtcNow &&
                    x.RefreshToken.RefreshToken == req.RefreshToken
                )
                .FirstOrDefaultAsync(ct) ?? throw new Exceptions.UnauthorizedException();

            return new BaseResponse<RefreshResponse>(
                new RefreshResponse()
                {
                    Token = JwtBearer.CreateToken(o =>
                    {
                        o.ExpireAt = DateTime.Now.AddMinutes(TokenConfig.Value.AccessTokenDurationMinutes);
                        o.User.Claims.Add((Constant.Claims.ID, data.ID.ToString()));
                        o.User.Claims.Add((Constant.Claims.Username, data.Username));
                        o.User.Claims.Add((Constant.Claims.Email, data.Email));
                    }),
                }
            );
        }
    }

    public class RefreshRequest
    {
        [FromHeader("X-Refresh-Token")]
        public required string RefreshToken { get; set; }
    }

    public class RefreshResponse
    {
        public required string Token { get; set; }
    }
}