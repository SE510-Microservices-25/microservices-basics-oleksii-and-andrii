using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PollSystem.Entities;
using VoteSystem.Consumer;
using VoteSystem.Data;
using VoteSystem.Repository;
using VoteSystem.Services;

var builder = WebApplication.CreateBuilder(args);
const string globalRouteAttribute = "votes";
var keycloakAuthority = builder.Configuration["Authentication:ValidIssuer"];
var keycloakClientId = builder.Configuration["Authentication:ClientID"];
var keycloakInternalUrl = builder.Configuration["Keycloak:InternalUrl"];

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect scope" },
                    { "profile", "User profile" },
                    { "email", "User email" }
                }
            }
        }
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Keycloak"
                }
            },
            new List<string> { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = builder.Configuration["Authentication:ClientID"];
        o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Authentication:ValidIssuer"],
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                using var httpClient = new HttpClient();
                var jwksUri = $"{keycloakInternalUrl}/realms/MyRealm/protocol/openid-connect/certs";
                var jwks = httpClient.GetStringAsync(jwksUri).Result;
                var keys = new JsonWebKeySet(jwks);
                return keys.GetSigningKeys().Where(k => k.KeyId == kid);
            }
        };
    });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<VoteCreatedConsumer>();
    x.AddConsumer<PollAddedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("vote-created-queue", e => { e.ConfigureConsumer<VoteCreatedConsumer>(context); });
        cfg.ReceiveEndpoint("poll-added-queue", e => { e.ConfigureConsumer<PollAddedConsumer>(context); });
    });
});

builder.Services.AddDbContext<VotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDbConnection")));
builder.Services.AddScoped<VotesRepository>();
builder.Services.AddScoped<VoteService>();
builder.Services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<VotesDbContext>();
        dbContext.Database.Migrate();
        Console.WriteLine("Database migrated successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = $"{globalRouteAttribute}/swagger/{{documentName}}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = $"{globalRouteAttribute}/swagger";
        options.SwaggerEndpoint($"/{globalRouteAttribute}/swagger/v1/swagger.json", "My API V1");
        options.OAuthClientId(keycloakClientId);
        options.OAuthAppName("My API - Swagger");
        options.OAuthUsePkce(); // Enables PKCE for security
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();