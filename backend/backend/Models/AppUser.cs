using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class AppUser:IdentityUser
    {
        [PersonalData]
        [Column(TypeName ="nvarchar(150)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string LastName { get; set; } = string.Empty;

        // Veritabanında "/uploads/profiles/resim_adi.jpg" şeklinde tutulacak
        [Column(TypeName = "nvarchar(MAX)")]
        public string? ProfileImageUrl { get; set; }
    }
}
