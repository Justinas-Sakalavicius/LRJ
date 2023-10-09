using Host.Endpoints;
using Host.Hubs;
using Host.Services;

var builder = WebApplication.CreateBuilder(args);

//application services
builder.Services.AddSignalR();
builder.Services.AddScoped<IEncodingService, EncodingService>();
builder.Services.AddHostedService<BackgroundJobService>();
builder.Services.AddMemoryCache();

// presentation services
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder
        .WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.MapEncodingEndpoints();
app.MapHub<JobProgressHub>("/api/encodinghub");

app.Run();