using Microsoft.EntityFrameworkCore;
using Scorm.Business.Adapters;
using Scorm.Business.Adapters.Abstract;
using Scorm.Business.Extensions;
using Scorm.Business.Services;
using Scorm.Business.Services.Abstract;
using Scorm.Core.Utilities.Settings;
using Scorm.Entities;
using Scorm.Repositories;
using Scorm.Repositories.Abstract;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<LRSContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("LRSDataBase"));
//});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});





builder.Services.AddRepositoryServices(builder.Configuration);
builder.Services.AddBusinessServices();
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("XApiAuth"));

// In-memory store (test amaçlý)
builder.Services.AddSingleton<StatementStore>();
builder.Services.AddSingleton<StateStore>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();

app.MapControllers();

app.Run();
