using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using TuneStore.Application.Abstractions;
using TuneStore.Application.DTOs;
using TuneStore.Domain.Entities;

namespace TuneStore.Infrastructure.Security
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _config;
        private object signingCredentials;

        public AuthService(IUnitOfWork uow, IDataProtectionProvider provider, IConfiguration config)
        {
            _uow = uow;
            _protector = provider.CreateProtector("TuneStore.PasswordProtector.v1");
            _config = config;
        }

        // Kullanıcı kayıt
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = (await _uow.Users.FindAsync(u => u.Email == request.Email)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = _protector.Protect(request.Password),
                Role = UserRole.Customer
            };

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return GenerateToken(user);
        }

        private AuthResponse GenerateToken(User user)
        {
            throw new NotImplementedException();
        }

        // Kullanıcı giriş
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = (await _uow.Users.FindAsync(u => u.Email == request.Email))
                       .FirstOrDefault() ?? throw new UnauthorizedAccessException("Invalid credentials");

            var plainPassword = _protector.Unprotect(user.Password);
            if (plainPassword != request.Password)
                throw new UnauthorizedAccessException("Invalid credentials");

            return GenerateToken(user);
        }

        private AuthResponse GenerateToken(User user, SigningCredentials creds)
        {
            return GenerateToken(user, creds, signingCredentials);
        }

        private AuthResponse GenerateToken(User user, SigningCredentials creds, object signingCredentials)
        {
            throw new NotImplementedException();
        }

        // JWT token üretimi
        private AuthResponse GenerateToken(User user, SigningCredentials creds, SigningCredentials signingCredentials, SigningCredentials tokenSigningCredentials)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var jwtTokenSigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresMinutes"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("name", $"{user.FirstName} {user.LastName}")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: jwtTokenSigningCredentials);

            return new AuthResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAtUtc = expires,
                Role = user.Role
            };
        }
    }
}
