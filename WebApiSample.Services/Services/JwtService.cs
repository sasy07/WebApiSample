using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApiSample.Entities;

namespace WebApiSample.Services.Services;

public class JwtService : IJwtService
{
    private readonly SignInManager<User> _signInManager;
    private readonly SiteSettings _siteSettings;

    public JwtService(IOptionsSnapshot<SiteSettings> setting, SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
        _siteSettings = setting.Value;
    }

    public  async Task <string> GenerateAsync(User user)
    {
        var secretKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.SecretKey);
        var signingCredentials =
            new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256);

        //Encryption JWT
        var encryptionKey = Encoding.UTF8.GetBytes(_siteSettings.JwtSettings.EncryptKey);
        var encryptionCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionKey),
            SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);


        var claims = await _getClaimsAsync(user);
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _siteSettings.JwtSettings.Issuer,
            Audience = _siteSettings.JwtSettings.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(0),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
            EncryptingCredentials = encryptionCredentials
        };

        // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        // JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
        // JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(descriptor);
        var jwt = tokenHandler.WriteToken(securityToken);
        return jwt;
    }

    private async Task<IEnumerable<Claim>> _getClaimsAsync(User user)
        => (await _signInManager.ClaimsFactory.CreateAsync(user)).Claims;

    //{
    // var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType; 
    // var list = new List<Claim>
    // {
    //     new Claim(ClaimTypes.Name, user.UserName),
    //     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //     new Claim(ClaimTypes.MobilePhone , "09163357023"),
    //     new Claim(securityStampClaimType , user.SecurityStamp.ToString())
    // };
    // var roles = _getRoles(user);
    // roles.ToList().ForEach(role =>
    // {
    //     list.Add(new Claim(ClaimTypes.Role, role.Name));
    // });
    // return list;
    //  }

    private Role[] _getRoles(User user)
    {
        return new Role[]
        {
            new Role
            {
                Name = "Admin"
            },
            new Role
            {
                Name = "PowerUser"
            }
        };
    }
}