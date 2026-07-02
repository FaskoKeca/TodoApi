using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain.Entities;
using TodoApi.Dtos;
using TodoApi.Providers;

namespace TodoApi.Controllers;

[ApiController]
[Route("api")]
public class TodoItemsController(ITodoItemProvider itemProvider) : ControllerBase
{
    //GET: /api/items/{id}
    
    [HttpGet("items/{id:int}")]
    public async Task<ActionResult<TodoItemDto>> GetById(int id)
    {
        var item = await itemProvider.GetByIdAsync(id);
        if (item == null) return BadRequest();
        var dto = new TodoItemDto
        {
            Id = item.Id,
            TodoListId = item.TodoListId,
            Title = item.Title,
            Notes = item.Notes,
            Priority = item.Priority,
            Due = item.Due,
            Status  = item.Status,
            TodoItemTags = item.TodoItemTags.Select(t => new TodoItemTagsDto()
            {
                TodoItemId = t.TodoItemId,
                TagId = t.TagId,
                Tag = t.Tag
            }).ToList()
        };
        return Ok(dto);
    }

    
    //GET: /api/lists/{listId}/items
    //optional query: status, overdue
    
    [HttpGet("lists/{listId:int}/items")]
    public async Task<ActionResult<List<TodoItemDto>>> GetByList(
        [Required]int listId,
        [FromQuery] TodoStatus? status)
    {
        var items = await itemProvider.GetByListIdAsync(listId, status);
        var dto = items.Select(list => new TodoItemDto
        {
            Id = list.Id,
            TodoListId = list.TodoListId,
            Title = list.Title,
            Notes = list.Notes,
            Priority = list.Priority,
            Due = list.Due,
            TodoItemTags = list.TodoItemTags
        });
        return Ok (dto);
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
        var created = await itemProvider.CreateAsync(
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

    public CancellationToken CancellationToken { get; set; }

    //PUT: /api/items/{id}/status
    

    [HttpPut("items/{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        TodoStatus status)
    {
        await itemProvider.UpdateStatusAsync(id, status);
        return NoContent();
    }
    
    //POST: /api/items/{id}/complete
    
    [HttpPost("items/{id}/complete")]
    public async Task<ActionResult> Complete(int id)
    {
        await itemProvider.UpdateStatusAsync(id, TodoStatus.Completed);
        return Ok();
    }
    
    
    //DELETE: /api/items/{id}
    
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await itemProvider.DeleteAsync(id);
        return NoContent();
    }
}