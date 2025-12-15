using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using EarthquakeShelter.Api.DTOs;
using EarthquakeModel;

namespace EarthquakeShelter.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AdminController(UserManager<ShelterUser> userManager, JwtHandler jwtHandler) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        ShelterUser? user = await userManager.FindByNameAsync(loginRequest.UserName);
        if (user == null)
        {
            return Unauthorized("Bad user name");
        }

        bool success = await userManager.CheckPasswordAsync(user, loginRequest.Password);
        if (!success)
        {
            return Unauthorized("Wrong password");
        }

        JwtSecurityToken secToken = await jwtHandler.GetTokenAsync(user);
        string? jwtstr = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult
        {
            Success = true,
            Message = "Cool",
            Token = jwtstr
        });
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegistrationRequest registrationRequest)
    {
        ShelterUser? existing = await userManager.FindByNameAsync(registrationRequest.UserName);
        if (existing != null) {
            return BadRequest("User already exists.");
        }

        ShelterUser newUser = new()
        {
            UserName = registrationRequest.UserName,
            Email = registrationRequest.Email
        };

        IdentityResult result = await userManager.CreateAsync(newUser, registrationRequest.Password);
        if (!result.Succeeded) {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        JwtSecurityToken secToken = await jwtHandler.GetTokenAsync(newUser);
        string? jwtstr = new JwtSecurityTokenHandler().WriteToken(secToken);
        return Ok(new LoginResult
        {
            Success = true,
            Message = "Registered",
            Token = jwtstr
        });
    }
}
