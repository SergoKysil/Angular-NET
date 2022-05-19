using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UserController : BaseController
{
    private readonly DataContext _context;
    
    public UserController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() => await _context.Users.AsNoTracking().ToListAsync();

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> Get(int id) => await _context.Users.FindAsync(id);
}