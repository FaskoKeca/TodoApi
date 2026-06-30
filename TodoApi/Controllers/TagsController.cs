using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
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

    //GET: /api/tags

    [HttpGet]
    public async Task<ActionResult<List<TagDto>>> GetAll()
    {
        var tags = await _provider.GetAllAsync();
        var result = tags.Select(t => new TagDto
        {
            Id = t.Id,
        }).ToList();

        return Ok(result);
    }

    //POST: /api/tags

    public record CreateTagRequest(string Name);

    [HttpPost]
    public async Task<ActionResult<TagDto>> Create(CreateTagRequest request)
    {
        try
        {
            var tag = await _provider.CreateAsync(request.Name);
            var dto = new TagDto
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return CreatedAtAction(
                nameof(GetAll),
                new { id = tag.Id },
                dto);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    //POST: /api/tags/merge

    public record MergeTagsRequest(int SourceTagId, int TargetTagId);

    [HttpPost("merge")]
    public async Task<ActionResult<MergeTagsResponse>> Merge(MergeTagsRequest request)
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

    //DELETE: /api/tags/{id}

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeleteTagResponse>> Delete(int id)
    {
        try
        {
            var affected = await _provider.DeleteAsync(id);

            return Ok(new DeleteTagResponse
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