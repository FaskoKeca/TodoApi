using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Dtos;
using TodoApi.Providers;
using TodoApi.Providers.Interfaces;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/lists")]
public class TodoListsController(ITodoListProvider provider) : ControllerBase
{
    //GET: /api/lists
    
    [HttpGet]
    public async Task<ActionResult<List<TodoListDto>>> GetAll()
    {
        var lists = await provider.GetAllAsync();
        return Ok (lists.Select(list => new TodoListDto
        {
            Id = list.Id,
            Name = list.Name
        }));
    }
    
    //GET: /api/lists/{id}
    
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetById(int id)
    {
        var list = await provider.GetByIdAsync(id);

        if (list == null)
            return NotFound();

        return Ok(new TodoListDto
        {
            Id =  list.Id,
            Name = list.Name
        });
    }
    
    //POST: /api/lists
    
    public record CreateTodoListRequest(string Name, string? Description);

    [HttpPost]
    public async Task<ActionResult<TodoListDto>> Create(CreateTodoListRequest request)
    {
        try
        {
            var created = await provider.CreateAsync(request.Name, request.Description);

            var dto = new TodoListDto
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description
            };

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //POST: /api/lists/{id}/archive
    
    [HttpPost("archive/{id}")]
    public async Task<IActionResult> Archive(int id)
    {
        try
        {
            await provider.ArchiveAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }
    
    //DELETE: /api/lists/{id}
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await provider.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}