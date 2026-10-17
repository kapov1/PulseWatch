using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("urls")]
public class UrlsController(UrlService urlService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult<UrlResponse> GetById(int id)
    {
        var result = urlService.FindById(id);

        if (result.IsFailure) return NotFound(result.ErrorMessage);

        UrlResponse response = new(result.Value!);
        return response;
    }

    [HttpGet("{name}")]
    public ActionResult<IEnumerable<UrlResponse>> GetByName(string name)
    {
        var urlModels = urlService.FindByName(name);
        if (!urlModels.Any()) return NotFound();

        IEnumerable<UrlResponse> urlResponses = urlModels.Select(url => new UrlResponse(url)); 
        return Ok(urlResponses);
    }

    [HttpPost]
    public ActionResult<UrlResponse> Create([FromBody] CreateUrlRequest request)
    {
        var result = urlService.AddUrl(request.Url, request.Name);

        if (result.IsFailure) return Conflict(result.ErrorMessage);

        UrlResponse response = new(result.Value!);

        return CreatedAtAction(nameof(GetByName), new { response.Name }, response);
    }

    [HttpPut("{id:int}")]
    public ActionResult<UrlResponse> Update(int id, [FromBody] CreateUrlRequest request)
    {
        var result = urlService.Update(request.Url, request.Name, id);

        if (result.IsFailure) return NotFound(result.ErrorMessage);

        var urlModel = result.Value!;
        urlModel.Update(request.Url, request.Name);

        return Ok(new UrlResponse(urlModel));
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var result = urlService.RemoveUrl(id);

        return result.IsSuccess ? NoContent() : NotFound(result.ErrorMessage);
    }
} 