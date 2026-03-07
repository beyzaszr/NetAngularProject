namespace backend.Models
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
}
