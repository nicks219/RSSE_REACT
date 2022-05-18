using Microsoft.AspNetCore.Mvc;

namespace RandomSongSearchEngine.Controllers;

public class TestController : Controller
{
    [HttpGet("test")]
    public ActionResult Get()
    {
        return Ok("from test controller");
    }
    
    [HttpGet("test.async")]
    public async Task<ActionResult> GetAsync()
    {
        await Task.Delay(1);
        return Ok("from async test controller");
    }
    
    [HttpGet("test.task")]
    public Task<ActionResult> GetTask()
    {
        return Task.FromResult<ActionResult>(Ok("from task test controller"));
    }
}