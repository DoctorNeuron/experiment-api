using ExperimentAPI;
using ExperimentAPI.DTO;
using ExperimentAPI.Endpoint;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<TokenConfiguration>(builder.Configuration.GetSection("Token"));

var tokenConfig = builder.Configuration.GetSection("Token").Get<TokenConfiguration>();

builder.Services.Configure<JwtCreationOptions>(o => o.SigningKey = tokenConfig!.Key);

builder.Services
    .AddCors(c =>
    {
        c.AddDefaultPolicy(p =>
        {
            p.AllowAnyHeader()
                .AllowAnyOrigin()
                .AllowAnyMethod();
        });
    })
    .AddAuthenticationJwtBearer(s => s.SigningKey = tokenConfig!.Key)
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();

builder.Services.AddDbContext<ExperimentDBContext>(op =>
{
    op.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});
builder.Services.AddScoped<ExperimentDBContext>();

var app = builder.Build();

app.UseHttpsRedirection()
    .UseCors()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(opt =>
    {
        opt.Endpoints.Configurator = ep =>
        {
            ep.PostProcessor<ExceptionPostProcessor>(Order.Before);
        };
        opt.Endpoints.RoutePrefix = "api";
        opt.Errors.ResponseBuilder = (failures, ctx, statusCode) =>
        {
            return new BaseResponse<Dictionary<string, List<string>>>(failures?
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(key => key.Key, value => value.Select(x => x.ErrorMessage).ToList()) ?? default!,
                "Validation Error",
                StatusCodes.Status400BadRequest
            );
        };
    })
    .UseOpenApi(c => c.Path = "/openapi/{documentName}.json");

app.MapScalarApiReference(x =>
{
    x.Authentication = new ScalarAuthenticationOptions()
    {
        PreferredSecuritySchemes = ["Bearer"]
    };
});

app.Run();