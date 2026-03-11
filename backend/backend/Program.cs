using backend.Controllers;
using backend.Extensions;
using backend.Models;
using backend.Services;
using Hangfire;
using Hangfire.SqlServer;
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

builder.Services.AddScoped<MeetingCleanupJob>();

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DevDB"), new SqlServerStorageOptions
    {
        // adjust options as needed
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
        SchemaName = "hangfire"
    });
});

builder.Services.AddHangfireServer();

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

app.UseHangfireDashboard("/hangfire");


app.MapControllers();

app.MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapGroup("/api")
    .MapIdentityUserEndpoints();


var recurringManager = app.Services.GetRequiredService<IRecurringJobManager>();
// Example: run every hour. Adjust Cron expression to your desired period.
recurringManager.AddOrUpdate<MeetingCleanupJob>(
    "delete-canceled-meetings",
    job => job.DeleteCanceledMeetings(),
    Cron.Minutely);



app.Run();

