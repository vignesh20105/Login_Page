var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/Vignesh", () => new
{
    User = "Vignesh",
    Status = "Active",
    ServerTime = DateTime.UtcNow
});

app.Run();