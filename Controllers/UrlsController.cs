using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("urls")]
public class UrlsController(UrlService urlService) : ControllerBase
{
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

    [HttpPut]
    public ActionResult<UrlResponse> Update([FromBody] string url, [FromBody] CreateUrlRequest request)
    {
        var result = urlService.FindUrlModel(url);

        if (result.IsFailure) return NotFound(result.ErrorMessage);

        var urlModel = result.Value!;
        urlModel.Update(request.Url, request.Name);

        return Ok(new UrlResponse(urlModel));
    }

    [HttpDelete]
    public IActionResult Delete([FromBody] string url)
    {
        var result = urlService.RemoveUrl(url);

        return result.IsSuccess ? NoContent() : NotFound(result.ErrorMessage);
    }
}

// git commit -m "feat: add UrlResponse, CreateUrlRequest, statesUrlController(empty), Urlscontroller(GET, POST, PUT, 