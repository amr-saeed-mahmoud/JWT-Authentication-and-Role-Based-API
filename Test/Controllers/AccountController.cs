using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Test.Data.Models;
using Test.DTOs;

namespace Test.Controllers;

[ApiController]
[Route("api/Account")]
public class AccountController : ControllerBase
{
    private UserManager<AppUser> _UserManager;
    private IConfiguration _Config;

    public AccountController(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _UserManager = userManager;
        _Config = configuration;
    }

    [AllowAnonymous]
    [HttpPost("SignIn", Name = "SignIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignIn(SignInDto UserInfo)
    {
        if(ModelState.IsValid && UserInfo.Password != null && !string.IsNullOrEmpty(UserInfo.Role))
        {
            AppUser User = new AppUser()
            {
                UserName = UserInfo.UserName,
                Email = UserInfo.Email,
                PhoneNumber = UserInfo.PhoneNumber
            };
            var Result = await _UserManager.CreateAsync(User, UserInfo.Password);
            await _UserManager.AddToRoleAsync(User, UserInfo.Role);
            if(Result.Succeeded)
            {
                return Ok("Succeeded");
            }
            else
            {
                foreach(var error in Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

        }
        return BadRequest(ModelState);
    }


    [AllowAnonymous]
    [HttpPost("LogIn", Name = "LogIn")]
    public async Task<IActionResult> LogIn(LogInDto UserInfo)
    {
       if(ModelState.IsValid)
       {    
            var User = await _UserManager.FindByNameAsync(UserInfo.UserName ?? "");
            if(User != null)
            {
                if(await _UserManager.CheckPasswordAsync(User, UserInfo.Password ?? ""))
                {
                    JwtSecurityToken TokenDescriptor = await CreateTokenDescriptor(User);
                    var token = new 
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(TokenDescriptor)
                    };
                    return Ok(token);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                ModelState.AddModelError("", "user name not exists.");
            }
       }
       return BadRequest(ModelState);
    }

    private async Task<JwtSecurityToken> CreateTokenDescriptor(AppUser User)
    {
        var roles = await _UserManager.GetRolesAsync(User);

        var Claims = new List<Claim>();
        Claims.Add(new Claim(ClaimTypes.NameIdentifier, User.Id!));
        Claims.Add(new Claim(ClaimTypes.Name, User.UserName!));
        Claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        // get the roles for this user
        var Roles = await _UserManager.GetRolesAsync(User);
        foreach(var role in Roles)
        {
            Claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }
        // add singing credentials
        var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Config["JWT:SecretKey"]!));
        var sc = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        var TokenDescriptor = new JwtSecurityToken(
            claims: Claims,
            issuer: _Config["JWT:Issuer"],
            audience: _Config["JWT:Audience"],
            signingCredentials: sc,
            expires: DateTime.Now.AddHours(1)
        );
        
        return TokenDescriptor;
    }
}