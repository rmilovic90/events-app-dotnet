using Events.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.ConfigureOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}

app.RegisterAllEndpoints();

app.Run();