var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7062")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddControllers();
var app = builder.Build();


//app.UseCors(builder => builder.WithOrigins("https://localhost:7062")
//                              .AllowAnyHeader()
//                              .AllowAnyMethod()
//                              );
//app.UseCors(builder => builder.AllowAnyOrigin());

app.MapGet("/", () => "Hello World!");

app.UseRouting();

app.UseCors();

app.MapControllers();

app.Run();
