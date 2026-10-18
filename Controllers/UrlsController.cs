using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("urls")]
public class UrlsController(UrlService urlService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public Results<Ok<UrlResponse>, NotFound<string>> GetById(int id)
    {
        var result = urlService.FindById(id);

        if (result.IsFailure) return TypedResults.NotFound(result.ErrorMessage);

        UrlResponse response = new(result.Value!);
        return TypedResults.Ok(response);
    }

    [HttpGet("{name}")]
    public Results<Ok<IEnumerable<UrlResponse>>, NotFound<string>> GetByName(string name)
    {
        var result = urlService.FindByName(name);
        if (result.IsFailure) return TypedResults.NotFound(result.ErrorMessage);

        var urlModels = result.Value!;

        IEnumerable<UrlResponse> urlResponses = urlModels.Select(url => new UrlResponse(url));
        return TypedResults.Ok(urlResponses);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateUrlRequest request)
    {
        var result = urlService.AddUrl(request.Url, request.Name);

        if (result.IsFailure) return Conflict(result.ErrorMessage);

        UrlResponse response = new(result.Value!);

        return CreatedAtAction(nameof(GetByName), new { response.Name }, response);
    }

    [HttpPut("{id:int}")]
    public Results<Ok<UrlResponse>, NotFound<string>> Update(int id, [FromBody] CreateUrlRequest request)
    {
        var result = urlService.Update(request.Url, request.Name, id);

        if (result.IsFailure) return TypedResults.NotFound(result.ErrorMessage);

        var urlModel = result.Value!;

        return TypedResults.Ok(new UrlResponse(urlModel));
    }

    [HttpDelete("{id:int}")]
    public Results<NoContent, NotFound<string>> Delete(int id)
    {
        var result = urlService.RemoveUrl(id);

        return result.IsSuccess ? 
            TypedResults.NoContent() : 
            TypedResults.NotFound(result.ErrorMessage);
    }
}