var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(builder => builder.WithOrigins("https://localhost:7062")
                              .AllowAnyHeader()
                              .AllowAnyMethod());

app.MapGet("/", () => "Hello World!");

app.MapPost("/test", () =>
{
    Dictionary<string, List<String>> info = new()
    {
        ["clients"] = new List<string>() { "Tom", "Bob", "Sam" },
        ["listeners"] = new List<string>() { "asdf", "sdfafsdas", "Sasdfafafsasm" }
    };
    return info;
});

app.UseRouting();
app.MapControllers();
app.Run();
