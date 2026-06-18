using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Infra.Security;

public class JwtTokenService
{
    private readonly JwtOptions _options;

    // --- INJEÇÃO DAS OPÇÕES DE JWT ---
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    // --- GERAR TOKEN JWT PARA UM OPERADOR ---
    public string GenerateToken(Operador operador)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, operador.Id.ToString()),
            new(ClaimTypes.NameIdentifier, operador.Id.ToString()),
            new(ClaimTypes.Name, operador.Nome),
            new(ClaimTypes.Role, operador.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
