using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDo.Data;
using ToDo.Models;
using ToDo.Models.ViewModels;

namespace ToDo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToDoController(ToDoDbContext context) : ControllerBase
{
    private readonly ToDoDbContext _context = context;
    private readonly HtmlSanitizer _htmlSanitizer = new();

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetTodos()
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
        {
            return Unauthorized();
        }

        var todos = await _context.Items
            .Where(i => i.UserId == user.Id)
            .ToListAsync();

        return Ok(todos.Select(t => new
        {
            t.Id,
            t.Task,
            t.IsCompleted
        }));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddTodo(ItemPostViewModel item)
    {
        var username = User.Identity?.Name;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null) return Unauthorized();
        item.Task = _htmlSanitizer.Sanitize(item.Task);
        var newItem = new Item
        {
            Task = item.Task,
            IsCompleted = false,
            UserId = user.Id
        };
        _context.Items.Add(newItem);
        await _context.SaveChangesAsync();
        return Ok(new
        {
            newItem.Id,
            newItem.Task,
            newItem.IsCompleted
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todo = await _context.Items.FindAsync(id);
        if (todo == null) return NotFound();

        _context.Items.Remove(todo);
        await _context.SaveChangesAsync();
        return Ok("Deleted");
    }
}