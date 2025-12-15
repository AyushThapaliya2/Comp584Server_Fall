using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace EarthquakeShelter.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using EarthquakeModel;

public class JwtHandler(IConfiguration configuration, UserManager<ShelterUser> userManager)
{
    public async Task<JwtSecurityToken> GetTokenAsync(ShelterUser user) =>
        new(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: await GetClaimsAsync(user),
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpirationTimeInMinutes"])),
            signingCredentials: GetSigningCredentials());

    private SigningCredentials GetSigningCredentials() {
        byte[] key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]!);
        SymmetricSecurityKey secret = new(key);
        return new(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync(ShelterUser user) {
        List<Claim> claims = [new(ClaimTypes.Name, user.UserName!)];
        claims.AddRange(from role in await userManager.GetRolesAsync(user) select new Claim(ClaimTypes.Role, role));
        return claims;
    }

}
