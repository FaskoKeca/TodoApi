using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
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
    
    //GET: /api/tags
    
    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetAll()
    {
        var tags = await _provider.GetAllAsync();
        return Ok(tags);
    }
    
    //POST: /api/tags
    
    public record CreateTagRequest(string Name);

    [HttpPost]
    public async Task<ActionResult<Tag>> Create(CreateTagRequest request)
    {
        try
        {
            var tag = await _provider.CreateAsync(request.Name);

            return CreatedAtAction(
                nameof(GetAll),
                new { id = tag.Id },
                tag);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    //POST: /api/tags/merge
    
    public record MergeTagsRequest(int SourceTagId, int TargetTagId);

    [HttpPost("merge")]
    public async Task<ActionResult<int>> Merge(MergeTagsRequest request)
    {
        try
        {
            var affected = await _provider.MergeAsync(
                request.SourceTagId,
                request.TargetTagId);

            return Ok(new { affectedItems = affected });
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
    
    //DELETE: /api/tags/{id}
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var affected = await _provider.DeleteAsync(id);

            return Ok(new
            {
                deletedTagId = id,
                affectedItems = affected
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}