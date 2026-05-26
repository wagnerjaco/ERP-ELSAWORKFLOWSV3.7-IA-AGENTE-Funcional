using ERP.Domain;
using ERP.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ERP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { message = "Usuário e senha são obrigatórios" });

            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioLogin == request.Usuario);
            if (usuario == null || !usuario.Ativo)
                return Unauthorized(new { message = "Usuário Inativo" });

            if (!BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
                return Unauthorized(new { message = "Usuário ou senha inválidos" });

            var token = GenerateJwtToken(usuario);
            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.UsuarioLogin,
                    usuario.Nome,
                    usuario.Email,
                    Permissao = usuario.Permissao.ToString()
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Senha))
                return BadRequest(new { message = "Usuário e senha são obrigatórios" });

            if (await context.Usuarios.AnyAsync(u => u.UsuarioLogin == request.Usuario))
                return BadRequest(new { message = "Usuário já existe" });

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                UsuarioLogin = request.Usuario,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Nome = request.Nome,
                Email = request.Email,
                Permissao = request.Permissao,
                Ativo = true,
                CreatedAt = DateTime.UtcNow
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();

            return Ok(new { message = "Usuário criado com sucesso", id = usuario.Id });
        }

        [HttpGet("health")]
        public IActionResult Health() => Ok(new { status = "Auth API rodando", time = DateTime.Now });

        private string GenerateJwtToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "ChaveSuperSecretaDoERPElsa2024PeloAmorDeDeus!@#$%"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.UsuarioLogin),
                new Claim(ClaimTypes.Email, usuario.Email ?? ""),
                new Claim(ClaimTypes.Role, usuario.Permissao.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"] ?? "ERPElsa",
                audience: configuration["Jwt:Audience"] ?? "ERPElsaAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public Permissao Permissao { get; set; } = Permissao.USUARIO;
    }
}