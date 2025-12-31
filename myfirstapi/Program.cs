using LoginApi.Services;  // Remove ProductApi.Data reference if not needed


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "My First Api", Version = "v1" });
});


var mongoSection = builder.Configuration.GetSection("MongoDb");
var connectionString = mongoSection["ConnectionString"];


if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine(" MongoDB ConnectionString not found in appsettings.json");
    
    
    connectionString = "mongodb://localhost:27017";
    Console.WriteLine($" Using local MongoDB: {connectionString}");
}


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


builder.Services.AddSingleton<MongoDbService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/mongo-connection", async (MongoDbService mongoService) =>
{
    try
    {
        var users = await mongoService.GetAsync();
        return Results.Ok(new 
        { 
            success = true,
            message = $" MongoDB connected! Found {users.Count} users.",
            usersCount = users.Count,
            timestamp = DateTime.UtcNow
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($" MongoDB error: {ex.Message}");
    }
});

app.MapGet("/", () => "Login API is running!");

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("ogin API Starting...");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"MongoDB: {connectionString}");

app.Run();