using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Providers;

namespace TodoApi.Controllers;

[ApiController]
[Route("api")]
public class TodoItemsController : ControllerBase
{
    private readonly ITodoItemProvider _itemProvider;

    public TodoItemsController(ITodoItemProvider itemProvider)
    {
        _itemProvider = itemProvider;
    }

    
    //GET: /api/items/{id}
    
    [HttpGet("items/{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var item = await _itemProvider.GetByIdAsync(id);
        return Ok(item);
    }

    
    //GET: /api/lists/{listId}/items
    //optional query: status, overdue
    
    [HttpGet("lists/{listId}/items")]
    public async Task<ActionResult<List<TodoItem>>> GetByList(
        int listId,
        [FromQuery] TodoStatus? status)
    {
        var items = await _itemProvider.GetByListIdAsync(listId, status);
        return Ok(items);
    }

    
    //POST: /api/lists/{listId}/items
    
    public record CreateTodoItemRequest(
        string Title,
        string? Notes,
        Priority Priority,
        DateTime? Due
    );

    [HttpPost("lists/{listId}/items")]
    public async Task<ActionResult<TodoItem>> Create(
        int listId,
        CreateTodoItemRequest request)
    {
        var created = await _itemProvider.CreateAsync(
            listId,
            request.Title,
            request.Notes,
            request.Priority,
            request.Due
        );

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            created
        );
    }
    
    //PUT: /api/items/{id}/status
    
    public record UpdateStatusRequest(TodoStatus Status);

    [HttpPut("items/{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateStatusRequest request)
    {
        await _itemProvider.UpdateStatusAsync(id, request.Status);
        return NoContent();
    }
    
    //POST: /api/items/{id}/complete
    
    [HttpPost("items/{id}/complete")]
    public async Task<ActionResult<TodoItem>> Complete(int id)
    {
        var item = await _itemProvider.CompleteAsync(id);
        return Ok(item);
    }
    
    //POST: /api/items/{id}/tags
    
    public record AssignTagsRequest(List<int> TagIds);

    [HttpPost("items/{id}/tags")]
    public async Task<ActionResult<TodoItem>> AssignTags(
        int id,
        AssignTagsRequest request)
    {
        var item = await _itemProvider.AssignTagsAsync(id, request.TagIds);
        return Ok(item);
    }
    
    //DELETE: /api/items/{id}
    
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _itemProvider.DeleteAsync(id);
        return NoContent();
    }
}