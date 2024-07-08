using MinimalAPICheatsheet;
using MinimalAPICheatSheet;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<NameService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adding Custom Middlewares Example
app.UseHttpLogging(); // Have to set the Logging level on the appsetings.json
app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (NameService nameService) =>
{
    // Logging Example
    app.Logger.LogInformation("/weatherforecast called");

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// ---------------------------------------------------------------------------------------------------------------------

// Status Code Example
app.MapGet("/StatusCode", (bool ok) => ok ? Results.Ok("Everything is Ok!") : Results.BadRequest("Bad Request!"));

// Routing Example
app.MapGet("/", get); // Using Function
app.MapPost("/", () => "Post called");
app.MapPut("/", () => "Put called");
app.MapDelete("/", () => "Delete called");

// Route with instantiation Example
var personHandler = new PersonHandler();
app.MapGet("/Persons", personHandler.HandleGet);

// Route Parameters Example
//app.MapGet("/Persons/{id}", personHandler.HandleGetById);

// Route Parameters with Contrains Example
app.MapGet("/Persons/{id:int}", personHandler.HandleGetById);

// Parameter Binding Example
// Person Json from body to Person object automatically
app.MapPost("Persons", (Person person) => person.FirstName +  ", " + person.LastName);

// ---------------------------------------------------------------------------------------------------------------------

app.Run();

// Local Functions Example
string get() => "Get called";

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
