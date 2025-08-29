using GymBro.API.Data;
using GymBro.API.DTOs;
using GymBro.API.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;



namespace GymBro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }


        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.User.SingleOrDefault(u => u.Email == dto.Email);
            if (user == null) { return Unauthorized("Неверный пароль"); }

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Неверный email или пароль");

            var token = GenerateJWTToken(user);
            return Ok(new { Token = token });
        }


        public string GenerateJWTToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
