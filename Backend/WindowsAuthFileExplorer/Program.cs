using Microsoft.AspNetCore.Authentication.Negotiate;
using WindowsAuthFileExplorer.ExceptionMiddleware;
using WindowsAuthFileExplorer.Services;
using WindowsAuthFileExplorer.Services.Impl;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(b =>
    {
        b.WithOrigins("http://localhost:4200")
            .WithOrigins("http://192.168.0.11:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options => { options.FallbackPolicy = options.DefaultPolicy; });

builder.Services.AddTransient<IFileManagementService, FileManagementService>();
builder.Services.AddTransient<ExceptionMiddlewareService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(options => options
    .WithOrigins("http://localhost:4200")
    .WithOrigins("http://192.168.0.11:4200")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddlewareService>();

app.MapControllers();

app.Run();