using Microsoft.AspNetCore.Mvc;
using TodoApi.Domain.Entities;
using TodoApi.Dtos;
using TodoApi.Providers;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly ITagProvider _provider;

    public TagsController(ITagProvider provider)
    {
        _provider = provider;
    }

    // GET: /api/tags
    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll()
    {
        var tags = await _provider.GetAllAsync();

        var result = tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name
        }).ToList();

        return Ok(result);
    }

    // POST: /api/tags
    public record CreateTagRequest(string Name);

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagRequest request)
    {
        try
        {
            var tag = await _provider.CreateAsync(request.Name);

            var dto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                TodoItemTags = tag.TodoItemTags.Select(t => new TodoItemTagsDto()
                {
                    TodoItemId = t.TodoItemId,
                    TagId = t.TagId,
                    TodoItem = t.TodoItem
                }).ToList()
            };

            return CreatedAtAction(nameof(GetAll), new { id = tag.Id }, dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    // POST: /api/tags/merge
    public record MergeTagsRequest(int SourceTagId, int TargetTagId);

    [HttpPost("merge")]
    public async Task<IActionResult> Merge([FromBody] MergeTagsRequest request)
    {
        try
        {
            var affected = await _provider.MergeAsync(
                request.SourceTagId,
                request.TargetTagId);

            return Ok(new MergeTagsResponse
            {
                AffectedItems = affected
            });
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
    
    
    public record AssignTagsToItemRequest(int TodoItemId, List<int> TagIds);

    [HttpPost("assign-to-item")]
    public async Task<IActionResult> AssignToItem([FromBody] AssignTagsToItemRequest request)
    {
        try
        {
            await _provider.AssignToItemAsync(request.TodoItemId, request.TagIds);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{tagId}")]
    public async Task<ActionResult> GetByTagIdAsync(int tagId)
    {
        var tag = await _provider.GetByIdAsync(tagId);
        /*var dto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            TodoItemTags = tag.TodoItemTags;
        };*/
        return Ok(tag);
    }

    // DELETE: /api/tags/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var affected = await _provider.DeleteAsync(id);

            return Ok(new
            {
                DeletedTagId = id,
                AffectedItems = affected
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}