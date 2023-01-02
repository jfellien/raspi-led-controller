using Microsoft.AspNetCore.Mvc;

namespace LedStripControllerApi.Controllers;

[ApiController]
public class HelpController : ControllerBase
{
    [HttpGet("", Name = "Ping")]
    public ActionResult Ping()
    {
        return Ok("I'm alive");
    }
}