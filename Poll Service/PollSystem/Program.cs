using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PollSystem.Consumers;
using PollSystem.Data;
using PollSystem.Entities;
using PollSystem.Repositories;
using PollSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
string dbConnectionString = builder.Configuration.GetConnectionString("PollDbConnection")!;
builder.Services.AddDbContext<AppDbContext>(
	options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString))
);

// Load keycloak configuration
string keycloakAuthority = "http://localhost:8080/realms/MyRealm";
string keycloakClientId = "dotnet-api";

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
	options =>
	{
		options.AddSecurityDefinition(
			"oauth2", new OpenApiSecurityScheme
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
			}
		);

		options.AddSecurityRequirement(
			new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "oauth2"
						}
					},
					new List<string> { "openid", "profile", "email" }
				}
			}
		);
	}
);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(
		options =>
		{
			options.Authority = keycloakAuthority;
			options.Audience = keycloakClientId;
			options.RequireHttpsMetadata = false;
		}
	);
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Register RabbitMqService
builder.Services.AddMassTransit(
	x =>
	{
		x.AddConsumer<PollCreatedConsumer>();

		x.UsingRabbitMq(
			(context, cfg) =>
			{
				cfg.Host(
					"rabbitmq", h =>
					{
						h.Username("guest");
						h.Password("guest");
					}
				);

				cfg.ReceiveEndpoint(
					"poll-created-queue", e =>
					{
						e.ConfigureConsumer<PollCreatedConsumer>(context);
					}
				);
			}
		);
	}
);
builder.Services.AddTransient<RabbitMqService>();
builder.Services.AddHostedService<OutboxProcessor>();

// Add services, repositories
builder.Services.AddScoped<PollsService>();
builder.Services.AddScoped<PollsRepository>();

// Register MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

// Automated DB migration
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var dbContext = services.GetRequiredService<AppDbContext>();
		dbContext.Database.Migrate();
		Console.WriteLine("Database migrated successfully!");
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while migrating the database.");
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(
		options =>
		{
			options.SwaggerEndpoint("/polls/swagger/v1/swagger.json", "Polls API V1");
			options.OAuthClientId(keycloakClientId);
			options.OAuthAppName("Polls API - Swagger");
			options.OAuthUsePkce();
		}
	);
}

app.MapGet(
	"/consumer", async () =>
	{
		var rabbitMqService = app.Services.GetRequiredService<RabbitMqService>();
		await rabbitMqService.SendMessage(
			new PollCreateDto()
			{
				Question = "Hello world? Build 06.04.2025",
				Options = new List<PollOptionCreateDto>()
				{
					new PollOptionCreateDto() { Text = "Yes" },
					new PollOptionCreateDto() { Text = "No" }
				}
			}
		);
	}
);

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();
