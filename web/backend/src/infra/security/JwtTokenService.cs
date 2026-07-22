using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LiveEventsTicket.Backend.Infra.Security;

public class JwtTokenService
{
    private readonly JwtOptions _options;

    // --- INJECAO DAS OPCOES DE CONFIGURACAO DO JWT ---
    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    // --- GERA UM TOKEN JWT ASSINADO PARA O USUARIO INFORMADO ---
    public string GenerateToken(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Role)
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
