var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/test", () => {
    Dictionary<string, List<String>> info = new Dictionary<string, List<String>>();
    info["clients"] = new List<string>() { "Tom", "Bob", "Sam" };
    info["listeners"] = new List<string>() { "asdf", "sdfafsdas", "Sasdfafafsasm" };
    return info;
});


app.Run();
