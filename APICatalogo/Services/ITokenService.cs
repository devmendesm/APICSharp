using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Services;

public interface ITokenService
{
    // Gera o token baseado nas claims do usuario
    JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config);

    string GenerateRefreshToken();

    // Vai extrair as claims do token expirado para o refresh gerar um novo token
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config);
}
