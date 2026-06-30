using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Dtos;
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
    public async Task<ActionResult<List<TodoItemDto>>> GetByList(
        int listId,
        [FromQuery] TodoStatus? status)
    {
        var items = await _itemProvider.GetByListIdAsync(listId, status);
        return Ok (items.Select(list => new TodoItemDto
        {
            Id = list.Id,
            TodoListId = list.TodoListId,
            TodoList =  list.TodoList,
            Title = list.Title,
            Notes = list.Notes,
            Priority = list.Priority,
            Due = list.Due,
            TodoItemTags = list.TodoItemTags
        }));
    }

    
    //POST: /api/lists/{listId}/items
    
    public record CreateTodoItemRequest(
        string Title,
        string? Notes,
        Priority Priority,
        DateTime? Due
    );

    [HttpPost("lists/{listId}/items")]
    public async Task<ActionResult<TodoItemDto>> Create(
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

        var dto = new TodoItemDto
        {
            TodoListId =  created.TodoListId,
            Title = created.Title,
            Notes = created.Notes,
            Priority = created.Priority,
            Due = created.Due
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = created.Id },
            dto
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
        var dto = new TodoItemDto
        {
            Id = item.Id,
            TodoListId = item.TodoListId,
            TodoList = item.TodoList,
            Title = item.Title,
            Notes = item.Notes,
            Priority = item.Priority,
            Due = item.Due,
            TodoItemTags = item.TodoItemTags
        };
        return Ok(dto);
    }
    
    //DELETE: /api/items/{id}
    
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _itemProvider.DeleteAsync(id);
        return NoContent();
    }
}