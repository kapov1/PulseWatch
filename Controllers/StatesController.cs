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
    public async Task<IResult> GetAllStates()
    {
        var checkResults = await monitoringService.SendRequestsAsync(urlStorage);

        if (!checkResults.Any()) return TypedResults.NotFound("You haven't added any URLs");

        return TypedResults.Ok(checkResults);
    }
}