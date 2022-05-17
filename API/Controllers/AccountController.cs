using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseController
{
    private readonly DataContext _context;

    public AccountController(DataContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register(RegisterDto input)
    {
        if (await UserExists(input.UserName))
            return BadRequest("User name is already taken!");
        
        using var hmac = new HMACSHA512();

        var user = new AppUser()
        {
            UserName = input.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);

        await  _context.SaveChangesAsync();

        return user;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AppUser>> Login(LoginDto input)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == input.UserName.ToLower());

        if (user == null) return Unauthorized("Invalid user name!");
        
        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input.Password));

        if (computedHash.Where((t, i) => t != user.PasswordHash[i]).Any())
        {
            return Unauthorized("Invalid password!");
        }
        return user;
    }

    private async Task<bool> UserExists(string userName)
    {
        return await _context.Users.AnyAsync(c => c.UserName == userName.ToLower());
    }
}