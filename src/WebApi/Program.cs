using Events.WebApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Events API v1");
    });
}

app.UseHttpsRedirection();
app.RegisterAllEndpoints();

app.Run();