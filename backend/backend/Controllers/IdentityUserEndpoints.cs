using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers
{

    public class UserRegistrationModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        // Angular tarafındaki file input'tan gelen dosyayı bu karşılar
        public IFormFile? ProfileImageUrl { get; set; }
    }

    public class UserLoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public static class IdentityUserEndpoints
    {

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {


            app.MapPost("/userRegister", CreateUser);

            app.MapPost("/userLogin",SignIn);



            return app;
        }




        private static async Task<IResult> CreateUser(UserManager<AppUser> userManager,
            IWebHostEnvironment env,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            string? dbPath = null;

            // 1. Dosya Kaydetme İşlemi
            if (userRegistrationModel.ProfileImageUrl != null && userRegistrationModel.ProfileImageUrl.Length > 0)
            {
                // wwwroot/uploads/profiles klasör yolunu oluştur
                var uploadPath = Path.Combine(env.WebRootPath, "uploads", "profiles");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // Benzersiz dosya ismi oluştur
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userRegistrationModel.ProfileImageUrl.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await userRegistrationModel.ProfileImageUrl.CopyToAsync(stream);
                }

                // Veritabanına yazılacak string yol
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
        }



        private static async Task<IResult> SignIn(UserManager<AppUser> userManager,
                [FromBody] UserLoginModel userLoginModel,
                IOptions<AppSettings> appSettings)
        {
            var user = await userManager.FindByEmailAsync(userLoginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, userLoginModel.Password))
            {
                var signInKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(appSettings.Value.JWTSecret)
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
        }


    }
}
