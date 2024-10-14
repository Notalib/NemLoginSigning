using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nota.SystemTest;

namespace NemLoginSigningWebApp.Controllers;

[ApiController]
[AllowAnonymous]
public class PingController : ControllerBase
{
    private readonly ISystemTester _logic;
    private readonly TimeSpan _timeout;

    public PingController(ISystemTester livenessLogic)
    {
        _logic = livenessLogic;
        _timeout = TimeSpan.FromSeconds(60);
    }

    [HttpGet]
    [Route("ping")]
    public IActionResult Ping()
    {
        return Ok(new SimplePingResultDTO { pong = DateTime.UtcNow.ToString("o") });
    }

    [HttpGet]
    [Route("pingMedium")]
    public async Task<IActionResult> PingMediumAsync(CancellationToken cancellationToken)
    {
        return Ok(await _logic.RunTests(SystemTestLevel.Medium, _timeout, cancellationToken));
    }

    [HttpGet]
    [Route("pingDeep")]
    public async Task<IActionResult> PingDeepAsync(CancellationToken cancellationToken)
    {
        return Ok(await _logic.RunTests(SystemTestLevel.Deep, _timeout, cancellationToken));
    }
}
