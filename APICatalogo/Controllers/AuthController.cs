using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, UserManager<ApplicationUser> userManager,
                            RoleManager<IdentityRole> roleManager, IConfiguration configuration,
                            ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);

        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation(1, "Função adicionada");
                return StatusCode(StatusCodes.Status200OK,
                        new ResponseDTO { Status = "Sucesso", Message = $"Função {roleName} adicionada com sucesso!" });
            }
            else
            {
                _logger.LogInformation(2, "Erro");
                return StatusCode(StatusCodes.Status400BadRequest,
                        new ResponseDTO { Status = "Erro", Message = $"Erro ao adicionar a nova {roleName} função" });
            }
        }

        return StatusCode(StatusCodes.Status400BadRequest,
                        new ResponseDTO { Status = "Erro", Message = "Função já existe!" });
    }

    [HttpPost]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"Usuario {user.Email} adicionado à função {roleName}");
                return StatusCode(StatusCodes.Status200OK,
                        new ResponseDTO { Status = "Sucesso", Message = $"Usuario {user.Email} adicionado à função {roleName}" });
            }
            else
            {
                _logger.LogInformation(1, $"Erro: não foi possível adicionar o usuario {user.Email} na função {roleName}");
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseDTO
                {
                    Status = "Erro",
                    Message = $"Erro: não foi possível adicionar o usuario {user.Email} na função {roleName}"
                });
            }
        }

        return BadRequest(new { error = $"Não foi possível encontrar o usuario {email}" });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelDTO modelDto)
    {
        // Tenta localizar o usuario, ! -> usado para dizer que tem certeza que não sera null
        var user = await _userManager.FindByNameAsync(modelDto.UserName);

        if (user is not null && await _userManager.CheckPasswordAsync(user, modelDto.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // "_" -> discard, utiliza quando nao se interessamos pelo retorno, pois ja esta sendo armazenado
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                                out int refreshTokenValidityInMinutes);

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });

        }
        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModelDTO modelDto)
    {
        var userExists = await _userManager.FindByNameAsync(modelDto.Username!);

        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseDTO { Status = "Erro", Message = "Usuario já existe!" });
        }

        ApplicationUser user = new()
        {
            Email = modelDto.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = modelDto.Username
        };

        var result = await _userManager.CreateAsync(user, modelDto.Password!);

        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                        new ResponseDTO { Status = "Erro", Message = "Criação de usuario falhou!" });
        }

        return Ok(new ResponseDTO { Status = "Sucesso", Message = "Usuario criado com sucesso!" });
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModelDTO modelDto)
    {
        if (modelDto is null)
        {
            return BadRequest("Requisição do cliente invalida!");
        }

        string? accessToken = modelDto.AccessToken
                               ?? throw new ArgumentNullException(nameof(modelDto));

        string? refreshToken = modelDto.RefreshToken
                               ?? throw new ArgumentNullException(nameof(modelDto));

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken!, _configuration);

        if (principal == null)
        {
            return BadRequest("Token de acesso/atualização inválido!");
        }

        string username = principal.Identity.Name;

        var user = await _userManager.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken
                         || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Token de acesso/atualização inválido!");
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken,
        });
    }

    [Authorize]
    [HttpPost]
    [Route("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            return BadRequest("Nome de usuario inválido!");

        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent();
    }
}
