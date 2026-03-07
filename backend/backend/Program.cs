using backend.Controllers;
using backend.Extensions;
using backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();


builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
               .AddIdentityAuth(builder.Configuration)
               .AddAppConfig(builder.Configuration);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .AddIdentityAuthMiddlewares()
    .ConfigureCORS(builder.Configuration);


app.MapControllers();

app.MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapGroup("/api")
    .MapIdentityUserEndpoints();




app.Run();

