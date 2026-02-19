using Events.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureSecurity(builder.Configuration);
builder.Services.ConfigureAppServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}
app.UseSecurity();
app.RegisterAllEndpoints();

app.Run();