using Microsoft.EntityFrameworkCore;
using PollSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
string dbConnectionString = builder.Configuration.GetConnectionString("PollDbConnection")!;
builder.Services.AddDbContext<AppDbContext>(
	options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString))
);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

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
	app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();
