var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddRouting(options
    => options.LowercaseUrls = true);

builder.Services.AddControllers();

//Todo: add logger and DbContext *Timers*

//Todo: features add work timers through message broker

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var application = builder.Build();

if (application.Environment.IsDevelopment())
{
    application.UseSwagger();
    application.UseSwaggerUI();
}
else
{
    application.UseHttpsRedirection();
}

application.UseAuthorization();

application.UseCors(policyBuilder =>
{
    policyBuilder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .AllowCredentials();
});

application.MapControllers();

application.Run();