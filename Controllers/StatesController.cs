using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("states")]
public class StatesController(MonitoringService monitoringService, UrlService urlService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CheckResult>> GetState(int id)
    {
        var result = urlService.FindById(id);
        if (result.IsFailure) return NotFound(result.ErrorMessage);

        var urlModel = result.Value!;

        var checkResult = await monitoringService.SendRequestAsync(urlModel.Url);

        return checkResult;
    }

    [HttpPost]
    public async Task<ActionResult<CheckResult>> GetState([FromBody] string url)
    {
        var checkResult = await monitoringService.SendRequestAsync(url);
        return checkResult;
    }
}