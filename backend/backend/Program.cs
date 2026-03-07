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

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = 
    x.DefaultChallengeScheme = 
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(y=>
{
    y.SaveToken = false;
    y.TokenValidationParameters =new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTSecret"]!))
    };
});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();

app.MapPost("/api/userRegister", async (
    UserManager<AppUser> userManager,
    IWebHostEnvironment env,
    [FromBody] UserRegistrationModel userRegistrationModel
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
    var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

    if (result.Succeeded)
        return Results.Ok(result);
    else
        return Results.BadRequest(result);
});

app.MapPost("/api/userLogin", async(
    UserManager < AppUser > userManager,
    [FromBody] UserLoginModel userLoginModel )=>
    {
        var user = await userManager.FindByEmailAsync(userLoginModel.Email);
        if (user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password))
        {
            var signInKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JWTSecret"]!)
                                    );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                 {
                     new Claim("UserID",user.Id.ToString())
                 }),
                Expires = DateTime.UtcNow.AddDays(10),
                SigningCredentials = new SigningCredentials(
                    signInKey,
                    SecurityAlgorithms.HmacSha256Signature
                    )
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Results.Ok(new { token });
        }
        else 
            return Results.BadRequest(new { message = "Username or password is incorrect." });
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

public class UserLoginModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}