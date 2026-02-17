using Events.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.ConfigureOpenApi();
builder.Services.ConfigureSecurity(builder.Configuration);

var app = builder.Build();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}
app.UseSecurity();
app.RegisterAllEndpoints();

app.Run();