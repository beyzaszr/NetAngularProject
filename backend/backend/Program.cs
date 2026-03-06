using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Config. CORS
app.UseCors(options =>
       options.WithOrigins("http://localhost:4200")
       .AllowAnyMethod()
       .AllowAnyHeader());
#endregion

app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/userRegister", async (
    UserManager<AppUser> userManager,
    IWebHostEnvironment env,
    [FromForm] UserRegistrationModel userRegistrationModel
    ) =>
{
    string? dbPath = null;

    // 1. Dosya Kaydetme Ýþlemi
    if (userRegistrationModel.ProfileImageUrl != null && userRegistrationModel.ProfileImageUrl.Length > 0)
    {
        // wwwroot/uploads/profiles klasör yolunu oluþtur
        var uploadPath = Path.Combine(env.WebRootPath, "uploads", "profiles");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Benzersiz dosya ismi oluþtur
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userRegistrationModel.ProfileImageUrl.FileName);
        var filePath = Path.Combine(uploadPath, fileName);

        // Dosyayý kaydet
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await userRegistrationModel.ProfileImageUrl.CopyToAsync(stream);
        }

        // Veritabanýna yazýlacak string yol
        dbPath = "/uploads/profiles/" + fileName;
    }

    AppUser user = new AppUser()
    {
        UserName = userRegistrationModel.Email,
        FirstName = userRegistrationModel.FirstName,
        LastName = userRegistrationModel.LastName,
        Email = userRegistrationModel.Email,
        PhoneNumber = userRegistrationModel.PhoneNumber,
        //Profile image eklenecek:
        ProfileImageUrl = dbPath
    };
    var result= await userManager.CreateAsync(user, userRegistrationModel.Password);

    if (result.Succeeded)
        return Results.Ok(result);
    else
        return Results.BadRequest(result);
});

app.Run();

public class UserRegistrationModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    // Angular tarafýndaki file input'tan gelen dosyayý bu karþýlar
    public IFormFile? ProfileImageUrl { get; set; }
}