using Lexicon.AuthService.Data;
using Lexicon.AuthService.Security;
using Lexicon.AuthService.Services;
using Lexicon.AuthService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Lexicon.AuthService.Implementation;
using Lexicon.Services.DataService.DbContext;
using Lexicon.AuthService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<LexiconDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "LexiconDatabase")));

builder.Services.AddScoped<PasswordHasher>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
