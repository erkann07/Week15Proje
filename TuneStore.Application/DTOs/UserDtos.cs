using System.ComponentModel.DataAnnotations;
using TuneStore.Domain.Entities;

namespace TuneStore.Application.DTOs
{
    public class RegisterRequest
    {
        [Required, StringLength(64)] public string FirstName { get; set; } = null!;
        [Required, StringLength(64)] public string LastName { get; set; } = null!;
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required, Phone] public string PhoneNumber { get; set; } = null!;
        [Required, MinLength(6)] public string Password { get; set; } = null!;
    }

    public class LoginRequest
    {
        [Required, EmailAddress] public string Email { get; set; } = null!;
        [Required] public string Password { get; set; } = null!;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public UserRole Role { get; set; }
    }
}
