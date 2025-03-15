using MassTransit;
using Microsoft.EntityFrameworkCore;
using VoteSystem.Consumer;
using VoteSystem.Data;
using VoteSystem.Models;
using VoteSystem.Services;

var builder = WebApplication.CreateBuilder(args);
// const string keycloakAuthority = "http://localhost:8080/realms/MyRealm";
// const string keycloakClientId = "dotnet-api";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

// options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
// {
//     Type = SecuritySchemeType.OAuth2,
//     Flows = new OpenApiOAuthFlows
//     {
//         AuthorizationCode = new OpenApiOAuthFlow
//         {
//             AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
//             TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
//             Scopes = new Dictionary<string, string>
//             {
//                 { "openid", "OpenID Connect scope" },
//                 { "profile", "User profile" },
//                 { "email", "User email" }
//             }
//         }
//     }
// });

// options.AddSecurityRequirement(new OpenApiSecurityRequirement
// {
//     {
//         new OpenApiSecurityScheme
//         {
//             Reference = new OpenApiReference
//             {
//                 Type = ReferenceType.SecurityScheme,
//                 Id = "oauth2"
//             }
//         },
//         new List<string> { "openid", "profile", "email" }
//     }
// });
// });

// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(o =>
//     {
//         o.RequireHttpsMetadata = false;
//         o.Audience = builder.Configuration["Authentication:Audience"];
//         o.MetadataAddress = builder.Configuration["Authentication:MetadataAddress"]!;
//         o.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidIssuer = builder.Configuration["Authentication:ValidIssuer"]
//         };
//     });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<VoteCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("vote-created-queue", e => { e.ConfigureConsumer<VoteCreatedConsumer>(context); });
    });
});

builder.Services.AddDbContext<VotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("VotesDbConnection")));
builder.Services.AddScoped<VoteService>();
builder.Services.AddTransient<RabbitMqService>();

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
    app.UseSwagger();
    app.UseSwaggerUI( /*options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        options.OAuthClientId(keycloakClientId);
        options.OAuthAppName("My API - Swagger");
        options.OAuthUsePkce(); // Enables PKCE for security
    }*/);
}

app.MapGet("/consumer", async () =>
{
    var rabbitMqService = app.Services.GetRequiredService<RabbitMqService>();
    await rabbitMqService.SendMessage(new Vote(1, 2, 3, 4, DateTime.UtcNow));
});

app.MapControllers();
app.UseHttpsRedirection();
app.Run();