using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Data.Models;

namespace Test.Controllers;

[Authorize]  // Require authentication for all actions in this controller
[ApiController]
[Route("api/Item")]
public class ItemController : ControllerBase
{

    private AppDbContext _db;

    public ItemController(AppDbContext db)
    {
        _db = db;
    }

    [Authorize(Roles = "Admin,User")]
    [HttpGet("Items", Name = "Items")]
    public async Task<IActionResult> GetItems()
    {
        var Items = await _db.Items.ToListAsync();
        return Ok(Items);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("Create", Name = "Create")]
    public async Task<IActionResult> CreateItem(Item NewItem)
    {
        await _db.Items.AddAsync(NewItem);
        await _db.SaveChangesAsync();
        return Ok(NewItem);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("Delete", Name = "Delete")]
    public async Task<IActionResult> DeleteItem(Item DeletedItem)
    {
        _db.Items.Remove(DeletedItem);
        await _db.SaveChangesAsync();
        return Ok(DeletedItem);
    }
}