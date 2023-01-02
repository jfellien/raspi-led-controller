using System.Drawing;
using LedStripControllerApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LedStripControllerApi.Controllers;

[ApiController]
[Route("led-strip")]
public class LedStripController : ControllerBase
{
    private readonly ILedStrip _ledStrip;
    private readonly ILogger<LedStripController> _logger;

    public LedStripController(
        ILedStrip ledStrip,
        ILogger<LedStripController> logger)
    {
        _ledStrip = ledStrip;
        _logger = logger;
    }

    [HttpPost("turn-on/{colorName}", Name = nameof(TurnOnWithColorName))]
    public ActionResult TurnOnWithColorName(string colorName)
    {
        Color color = Color.Empty;

        try
        {
           color = Color.FromName(colorName);
        }
        catch
        {
            string message = $"unknown color {colorName}";

            _logger.LogError(message);

            return BadRequest(message);
        }

        _logger.LogInformation("LedStrip turns on with color {0}", colorName);

        _ledStrip.TurnOn(color);

        return Ok();
    }

    [HttpPost("turn-off", Name = nameof(TurnOff))]
    public ActionResult TurnOff()
    {
        _logger.LogInformation("LedStrip turns off");

        _ledStrip.TurnOff();

        return Ok();
    }

    [HttpPost("rainbow", Name = nameof(Rainbow))]
    public ActionResult Rainbow()
    {
        _logger.LogInformation("Show rainbow colors");

        _ledStrip.Rainbow();

        return Ok();
    }

    [HttpPost("rainbow/scroll-ascending", Name = nameof(RainbowScrollAscending))]
    public ActionResult RainbowScrollAscending()
    {
        _logger.LogInformation("Show rainbow colors ascending order");

        _ledStrip.RainbowAscending();

        return Ok();
    }

    [HttpPost("rainbow/scroll-descending", Name = nameof(RainbowDescending))]
    public ActionResult RainbowDescending()
    {
        _logger.LogInformation("Show rainbow colors descending order");

        _ledStrip.RainbowDescending();

        return Ok();
    }

    [HttpPost("random-color", Name = nameof(RandomColor))]
    public ActionResult RandomColor()
    {
        _logger.LogInformation("Show a random color");

        _ledStrip.RandomColor();

        return Ok();
    }

    [HttpPost("strobo/{durationInSeconds}", Name = nameof(Strobo))]
    public async Task<ActionResult> Strobo(int durationInSeconds)
    {
        _logger.LogInformation("Show strobo for {0} seconds", durationInSeconds);

        await _ledStrip.Strobo(durationInSeconds, CancellationToken.None);

        return Accepted();
    }

    [HttpPost("strobo/{durationInSeconds}/{onTimeInMilliseconds}/{offTimeInMilliseconds}", Name = nameof(StroboControlled))]
    public async Task<ActionResult> StroboControlled(int durationInSeconds, int onTimeInMilliseconds, int offTimeInMilliseconds)
    {
        _logger.LogInformation("Show strobo for {0} seconds and timings on: {1} ms, off: {2} ms", 
                durationInSeconds, 
                onTimeInMilliseconds, 
                offTimeInMilliseconds);

        await _ledStrip.Strobo(durationInSeconds, onTimeInMilliseconds, offTimeInMilliseconds, CancellationToken.None);

        return Accepted();
    }

    [HttpPost("strobo/random-color/{durationInSeconds}", Name = nameof(RandomStrobo))]
    public async Task<ActionResult> RandomStrobo(int durationInSeconds)
    {
        _logger.LogInformation("Show strobo for {0} seconds", durationInSeconds);

        await _ledStrip.RandomStrobo(durationInSeconds, CancellationToken.None);

        return Accepted();
    }

    [HttpPost("knight-rider/{colorName}/{loops}/{lengthOfLights}", Name = nameof(KnightRider))]
    public async Task<ActionResult> KnightRider(string colorName, int loops, int lengthOfLights)
    {
        Color color = Color.Empty;

        try
        {
           color = Color.FromName(colorName);
        }
        catch
        {
            string message = $"unknown color {colorName}";

            _logger.LogError(message);

            return BadRequest(message);
        }

        if(lengthOfLights > _ledStrip.NumberOfLeds)
        {
            string message = $"Too many lights requested {lengthOfLights}. The strip only can less than {_ledStrip.NumberOfLeds}.";

            _logger.LogError(message);

            return BadRequest(message);
        }

        _logger.LogInformation("Show knight rider for {0} times, color {1} and leght of light {2}", loops, colorName, lengthOfLights);

        await _ledStrip.KnightRider(color, loops, lengthOfLights, CancellationToken.None);

        return Accepted();
    }
}