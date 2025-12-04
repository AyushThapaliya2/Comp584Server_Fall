using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using WorldModel;

using Comp_584_Server.DTOs;

namespace Comp_584_Server;

[Route("api/[controller]")]
[ApiController]
public class AdminController(UserManager<WorldModelUser> userManager, JwtHandler jwtHandler) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        WorldModelUser? user = await userManager.FindByNameAsync(loginRequest.UserName);
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
}