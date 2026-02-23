using Events.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureSecurity(builder.Configuration);
builder.Services.ConfigureAppServices(builder.Configuration);

var app = builder.Build();
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}
app.UseSecurity();
app.RegisterAllEndpoints();

app.Run();