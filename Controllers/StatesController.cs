using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("states")]
public class StatesController(MonitoringService monitoringService, UrlService urlService, UrlStorage urlStorage) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CheckResult>> GetState(int id)
    {
        var result = urlService.FindById(id);
        if (result.IsFailure) return NotFound(result.ErrorMessage);

        var urlModel = result.Value!;

        var checkResult = await monitoringService.SendRequestAsync(urlModel.Url);

        return Ok(checkResult);
    }

    [HttpPost]
    public async Task<ActionResult<CheckResult>> GetState([FromBody][UrlAddress] string url)
    {
        var checkResult = await monitoringService.SendRequestAsync(url);
        return Ok(checkResult);
    }

    [HttpGet]
    public async Task<ActionResult<List<CheckResult>>> GetAllStates()
    {
        List<Task<CheckResult>> results = new();

        foreach(var urlModel in urlStorage.Urls)
        {
            results.Add(monitoringService.SendRequestAsync(urlModel.Url));
        }

        return Ok(await Task.WhenAll(results));
    }
}